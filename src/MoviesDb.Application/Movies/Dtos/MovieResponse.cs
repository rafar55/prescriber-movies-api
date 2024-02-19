namespace MoviesDb.Application.Movies.Dtos;

public record MovieResponse(Guid Id, string Slug, string Title, int YearOfRelease, IEnumerable<string> Genres);

