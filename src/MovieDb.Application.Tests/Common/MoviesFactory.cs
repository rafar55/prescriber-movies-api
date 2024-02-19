using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Domain.Models;

namespace MovieDb.Application.Tests.Common;

public class MoviesFactory
{
    public Movie CreateMovie()
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            YearOfRelease = 2021,
            Title = "Test Movie",      
            Genres = ["action", "comedy"],          
        };
    }
}
