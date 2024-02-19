using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Movies.Mappers;

public static class MovieMappings
{
    public static MovieResponse ToResponse(this Movie movie)
    {
        return new MovieResponse(movie.Id, movie.Slug, movie.Title, movie.YearOfRelease, movie.Genres);       
    }

    public static IEnumerable<MovieResponse> ToResponseList(this IEnumerable<Movie> movies)
    {
        return movies.Select(ToResponse);
    }

    public static Movie MapToMovie(this CreateMovieRequest movie)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres.ToList(),           
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest movie, Guid movieId)
    {
        return new Movie
        {
            Id = movieId,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres.ToList()
        };
    }
}
