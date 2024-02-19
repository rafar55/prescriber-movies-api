using FluentValidation;
using MoviesDb.Application.Common.Dtos;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Application.Movies.Mappers;
using MoviesDb.Application.Movies.Validators;
using MoviesDb.Domain.Exceptions;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Movies.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _validator;
    private readonly TimeProvider _timeProvider;

    public MovieService(
        IMovieRepository movieRepository,
        IValidator<Movie> validator,
        TimeProvider timeProvider)
    {
        _movieRepository = movieRepository;
        _validator = validator;
        _timeProvider = timeProvider;
    }

    public async Task<MovieResponse> GetMovieAsync(Guid movieId)
    {
        var movieDb = await _movieRepository.GetMovieByIdAsync(movieId);
        if (movieDb == null)
        {
            throw new EntityNotFoundException(movieId, nameof(Movie));
        }
        return movieDb.ToResponse();
    }

    public async Task<PagedResponse<MovieResponse>> GetMoviesAsync(GetMoviesListRequest request)
    {
        var pagedResponse = await _movieRepository.GetMoviesAsync(request);
        return new PagedResponse<MovieResponse>
        {
            PageSize = pagedResponse.PageSize,
            Page = pagedResponse.Page,
            Total = pagedResponse.Total,
            Items = pagedResponse.Items.ToResponseList()
        };
    }

    public async Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request, Guid userId)
    {
        var dbModel = request.MapToMovie();
        dbModel.CreatedBy = userId;
        dbModel.CreatedAt = _timeProvider.GetUtcNow();

        await _validator.ValidateAndThrowAsync(dbModel);

        await _movieRepository.AddMovieAsync(dbModel);
        return dbModel.ToResponse();
    }

    public async Task<MovieResponse> UpdateMovieAsync(Guid movieId, UpdateMovieRequest request)
    {
        var dbModel = request.MapToMovie(movieId);
        await _validator.ValidateAndThrowAsync(dbModel);

        var changeCount = await _movieRepository.UpdateMovie(dbModel);
        if(changeCount == 0)
        {
            throw new EntityNotFoundException(movieId, nameof(Movie));
        }

        return dbModel.ToResponse();
    }

    public async Task<bool> DeleteMovieAsync(Guid movieId)
    {
        var changeCount = await _movieRepository.DeleteMovieAsync(movieId);
        if(changeCount == 0)
        {
            throw new EntityNotFoundException(movieId, nameof(Movie));
        }
        return true;
    }


}
