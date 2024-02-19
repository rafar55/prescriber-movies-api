namespace MoviesDb.Application.Movies.Dtos;

public record UpdateMovieRequest(string Title, int YearOfRelease, IEnumerable<string> Genres);
