namespace MoviesDb.Application.Movies.Dtos;

public record CreateMovieRequest(string Title, int YearOfRelease, IEnumerable<string> Genres);
