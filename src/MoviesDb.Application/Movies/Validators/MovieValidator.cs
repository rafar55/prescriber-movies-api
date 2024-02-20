using FluentValidation;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Movies.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;

    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(250);
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.Year);
        RuleFor(x => x.Genres)
            .NotEmpty();

        RuleForEach(x => x.Genres)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(350)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists on our database");
    
    }

    private async Task<bool> ValidateSlug(string slug, CancellationToken token)
    {
        var result = await _movieRepository.ExistsSlugAsync(slug);
        return !result;

    }
}
