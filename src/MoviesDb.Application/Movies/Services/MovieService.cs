using MoviesDb.Application.Common.Dtos;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Movies.Dtos;

namespace MoviesDb.Application.Movies.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<MovieResponse> GetMovieAsync(Guid movieId)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<MovieResponse>> GetMoviesAsync(GetMoviesListRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<MovieResponse> UpdateMovieAsync(Guid movieId, UpdateMovieRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteMovieAsync(Guid movieId)
    {
        throw new NotImplementedException();
    }

   
}
