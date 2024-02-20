using MoviesDb.Application.Common.Dtos;
using MoviesDb.Application.Movies.Dtos;

namespace MoviesDb.Application.Movies.Services;

public interface IMovieService
{
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request, Guid userId);
    Task<bool> DeleteMovieAsync(Guid movieId);
    Task<MovieResponse> GetByIdAsync(Guid movieId);
    Task<MovieResponse> GetByIdOrSlugAsync(string idOrSlug);
    Task<MovieResponse> GetBySlugAsync(string slug);
    Task<PagedResponse<MovieResponse>> GetMoviesAsync(GetMoviesListRequest request);
    Task<MovieResponse> UpdateMovieAsync(Guid movieId, UpdateMovieRequest request);
}
