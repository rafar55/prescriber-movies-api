using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Common.Interfaces;

public interface IMovieRepository
{
    Task<int> AddMovieAsync(Movie newMovie);
    Task<Movie?> GetMovieByIdAsync(Guid movieId);
    Task<Movie?> GetMovieBySlugAsync(string slug);
    Task<IEnumerable<Movie>> GetMoviesAsync(GetMoviesListRequest request);
    Task<int> UpdateMovie(Movie movie);
}
