using Microsoft.Data.SqlClient;
using MoviesDb.Application.Common.Dtos;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Domain.Models;
using MoviesDb.Infrastructure.Database;
using System.Data;

namespace MoviesDb.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Movie?> GetMovieByIdAsync(Guid movieId)
    {
        var sql = """
            SELECT M.Id,M.Slug, M.Title, M.YearOfRelease, M.CreatedAt, M.CreatedBy,STRING_AGG(G.Name, ',') AS Genres
            FROM Movies AS M
            JOIN Genres AS G ON M.Id = G.MovieId
            WHERE Id = @movieId
            GROUP BY M.Id,M.Slug,M.Title, M.YearOfRelease, M.CreatedAt, M.CreatedBy
            """;

        //All this boiler plate code could be avoided by using Dapper
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@movieId", movieId);

        using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (!await reader.ReadAsync())
        {
            return null;
        }

        var movie = GetMovieFromReader(reader);
        return movie;
    }

    public async Task<bool> ExistsSlugAsync(string slug)
    {
        var sql = "SELECT 1 FROM Movies WHERE Id = @slug";
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@movieId", slug);

        return await command.ExecuteScalarAsync() is not null;
    }

    public async Task<Movie?> GetMovieBySlugAsync(string slug)
    {
        var sql = """
            SELECT M.Id,M.Slug,M.Title, M.YearOfRelease, M.CreatedAt, M.CreatedBy,STRING_AGG(G.Name, ',') AS Genres
            FROM Movies AS M
            JOIN Genres AS G ON M.Id = G.MovieId
            WHERE Slug = @slug
            GROUP BY M.Id,M.Title,M.Slug, M.YearOfRelease, M.CreatedAt, M.CreatedBy
            """;
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@slug", slug);

        using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (!await reader.ReadAsync())
        {
            return null;
        }

        var movie = GetMovieFromReader(reader);
        return movie;
    }

    public async Task<PagedResponse<Movie>> GetMoviesAsync(GetMoviesListRequest request)
    {

        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var countSql = """
            SELECT COUNT(movies.Id) FROM Movies 
            WHERE Title LIKE '%' + COALESCE(@title,'') + '%'
            AND @YearOfRelease IS NULL OR YearOfRelease = @YearOfRelease
            """;
        using var countCommand = new SqlCommand(countSql, (SqlConnection)connection);
        countCommand.Parameters.AddWithValue("@title", request.Title ?? (object)DBNull.Value);
        countCommand.Parameters.AddWithValue("@YearOfRelease", request.YearOfRelease ?? (object)DBNull.Value);
        var totalRecords = (await countCommand.ExecuteScalarAsync() as int?) ?? 0;
      
        if(totalRecords == 0)
        {
            return  PagedResponse<Movie>.Empty(request.PageSize, request.Page);
        }


        var sql = """
            SELECT M.Id,M.Slug, M.Title, M.YearOfRelease, M.CreatedAt, M.CreatedBy,STRING_AGG(R.Name, ',') AS Genres
            FROM Movies AS M
            JOIN Genres AS R ON M.Id = R.MovieId
            WHERE M.Title LIKE '%' + COALESCE(@title,'') + '%'
            AND @YearOfRelease IS NULL OR M.YearOfRelease = @YearOfRelease
            GROUP BY M.Id,M.Slug, M.Title, M.YearOfRelease, M.CreatedAt, M.CreatedBy            
            ORDER BY M.YearOfRelease DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROW ONLY OPTION (RECOMPILE)
            """;
        using var command = new SqlCommand(sql, (SqlConnection)connection);
        command.Parameters.AddWithValue("@title", request.Title ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@YearOfRelease", request.YearOfRelease ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@PageSize", request.PageSize);
        command.Parameters.AddWithValue("@Offset", request.GetOffset());

        using var reader = await command.ExecuteReaderAsync();
        var movies = new List<Movie>();
        while (await reader.ReadAsync())
        {
            movies.Add(GetMovieFromReader(reader));
        }

        return new PagedResponse<Movie>()
        {
            Items = movies,
            TotalItems = totalRecords,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<int> AddMovieAsync(Movie newMovie)
    {
        var sql = """
            INSERT INTO Movies (Id,Slug, Title, YearOfRelease, CreatedAt, CreatedBy)
            VALUES (@Id,@Slug, @Title, @YearOfRelease, @CreatedAt, @CreatedBy)
            """;
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = (SqlTransaction)connection.BeginTransaction();
        using var command = new SqlCommand(sql, (SqlConnection)connection, transaction);

        command.Parameters.AddWithValue("@Id", newMovie.Id);
        command.Parameters.AddWithValue("@Title", newMovie.Title);
        command.Parameters.AddWithValue("@YearOfRelease", newMovie.YearOfRelease);
        command.Parameters.AddWithValue("@CreatedAt", newMovie.CreatedAt);
        command.Parameters.AddWithValue("@CreatedBy", newMovie.CreatedBy);
        command.Parameters.AddWithValue("@Slug", newMovie.Slug);

        var result = await command.ExecuteNonQueryAsync();

        
        await AddGenresAsync(newMovie, connection, transaction);

        transaction.Commit();

        return result;
    }  

    public async Task<int> UpdateMovie(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = (SqlTransaction)connection.BeginTransaction();
        
        await RemoveMovieGenres(movie.Id, connection, transaction);
        await AddGenresAsync(movie, connection, transaction);

        var updateSql = """
            UPDATE Movies
            SET Title = @Title, YearOfRelease = @YearOfRelease, Slug = @Slug
            WHERE Id = @Id
            """;
        using var updateMovieCommand = new SqlCommand(updateSql, (SqlConnection)connection, transaction);
        updateMovieCommand.Parameters.AddWithValue("@Id", movie.Id);
        updateMovieCommand.Parameters.AddWithValue("@Title", movie.Title);
        updateMovieCommand.Parameters.AddWithValue("@Slug", movie.Slug);
        updateMovieCommand.Parameters.AddWithValue("@YearOfRelease", movie.YearOfRelease);
        var result = await updateMovieCommand.ExecuteNonQueryAsync();

        transaction.Commit();

        return result;
    }   

    public async Task<int> DeleteMovieAsync(Guid movieId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = (SqlTransaction)connection.BeginTransaction();
        await RemoveMovieGenres(movieId, connection, transaction);

        var deleteSql = "DELETE FROM Movies WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(deleteSql, (SqlConnection)connection, transaction);
        deleteCommand.Parameters.AddWithValue("@Id", movieId);
        var result = await deleteCommand.ExecuteNonQueryAsync();

        return result;
    }


    private static async Task RemoveMovieGenres(Guid movieId, IDbConnection connection, SqlTransaction transaction)
    {
        var deleteGenresSql = "DELETE FROM Genres WHERE MovieId = @MovieId";
        var deleteGenresCommand = new SqlCommand(deleteGenresSql, (SqlConnection)connection, transaction);
        deleteGenresCommand.Parameters.AddWithValue("@MovieId", movieId);
        await deleteGenresCommand.ExecuteNonQueryAsync();       
    }

    private static async Task AddGenresAsync(Movie newMovie, IDbConnection connection, SqlTransaction transaction)
    {
        //Now I insert the genres of the movie I am in single transaction
        //TODO I should use bulk insert for this maybe using datatable I need to check. I have been a long time without using ADO.NET directly
        foreach (var genre in newMovie.Genres)
        {
            var genresSql = "INSERT INTO Genres (MovieId, Name) VALUES (@MovieId, @Name)";
            using var genresCommand = new SqlCommand(genresSql, (SqlConnection)connection, transaction);
            genresCommand.Parameters.AddWithValue("@MovieId", newMovie.Id);
            genresCommand.Parameters.AddWithValue("@Name", genre);
            await genresCommand.ExecuteNonQueryAsync();
        }
    }

    private static Movie GetMovieFromReader(SqlDataReader reader)
    {
        var movie = new Movie
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            YearOfRelease = reader.GetInt32(reader.GetOrdinal("YearOfRelease")),
            CreatedAt = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedAt")),
            CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
            Genres = Enumerable.ToList(reader.GetString(reader.GetOrdinal("Genres")).Split(","))
        };
        return movie;
    }
}
