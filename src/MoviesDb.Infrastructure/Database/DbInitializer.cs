using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Movies.Services;
using MoviesDb.Domain.Models;

namespace MoviesDb.Infrastructure.Database;

public class DbInitializer
{
    private IMigrationRunner _migrationRunner;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IMovieRepository _movieRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public DbInitializer(
        IMigrationRunner migrationRunner,
        IDbConnectionFactory dbConnectionFactory,
        IMovieRepository movieRepository,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository)
    {
        _migrationRunner = migrationRunner;
        _dbConnectionFactory = dbConnectionFactory;
        _movieRepository = movieRepository;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    public async Task RunMigrationAsync()
    {
        await CreateDatabaseAsync("MoviesDb");
        _migrationRunner.MigrateUp();
    }

    public async Task SeedDatabaseAsync()
    {
        var cnn = await _dbConnectionFactory.CreateConnectionAsync();

        var userQuery = "SELECT 1 FROM Users";
        using var userCommand = new SqlCommand(userQuery, (SqlConnection)cnn);
        var usersExists = (await userCommand.ExecuteScalarAsync()) is not null;
        var adminUserId = Guid.Parse("00c84a49-483f-4c05-ad84-26772d18ab22");
        if (!usersExists)
        {
            var adminUser = new User()
            {
                Id = adminUserId,
                Email = "admin@supersecret.com",
                IsAdmin = true,
                PasswordHash = _passwordHasher.HashPassword("superadmin"),
                CreatedAt = DateTimeOffset.UtcNow,
                FirstName = "Super",
                LastName = "Admin",
            };

            await _userRepository.CreateAsync(adminUser);
        }


        var query = "SELECT 1 FROM Movies";
        using var sqlCommand = new SqlCommand(query, (SqlConnection)cnn);
        var moviesExists = (await sqlCommand.ExecuteScalarAsync()) is not null;
        if (!moviesExists)
        {
            var movies = new[]
            {
                new Movie()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Shawshank Redemption",
                    YearOfRelease = 1994,
                    Genres = new List<string> { "Drama" },
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = adminUserId
                },
                new Movie()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Godfather",
                    YearOfRelease = 1972,
                    Genres = new List<string> { "Crime", "Drama" },
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = adminUserId
                },
                new Movie()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Dark Knight",
                    YearOfRelease = 2008,
                    Genres = new List<string> { "Action", "Crime", "Drama" },
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = adminUserId
                }
            };
            await Parallel.ForEachAsync(movies, async (movie, _) => await _movieRepository.AddMovieAsync(movie));
        }
    }

    private async Task CreateDatabaseAsync(string DbName)
    {
        var query = "SELECT 1 FROM sys.databases WHERE name = @name";
        using var sqlConnection = (SqlConnection)await _dbConnectionFactory.CreateMasterConnection();
        using var sqlCommand = new SqlCommand(query, sqlConnection);
        sqlCommand.Parameters.AddWithValue("@name", DbName);
        var exists = (await sqlCommand.ExecuteScalarAsync()) is not null;
        if (!exists)
        {
            sqlCommand.CommandText = $"CREATE DATABASE {DbName}";
            await sqlCommand.ExecuteNonQueryAsync();
        }
    }
}
