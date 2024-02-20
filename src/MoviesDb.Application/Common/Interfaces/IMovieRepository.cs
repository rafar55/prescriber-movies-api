using MoviesDb.Application.Common.Dtos;
using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Common.Interfaces;

public interface IMovieRepository
{
    Task<int> AddMovieAsync(Movie newMovie);
    Task<int> DeleteMovieAsync(Guid movieId);
    Task<bool> ExistsSlugAsync(string slug);
    Task<Movie?> GetMovieByIdAsync(Guid movieId);
    Task<Movie?> GetMovieBySlugAsync(string slug);
    Task<PagedResponse<Movie>> GetMoviesAsync(GetMoviesListRequest request);
    Task<int> UpdateMovie(Movie movie);
}
