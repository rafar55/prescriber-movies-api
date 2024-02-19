using MoviesDb.Application.Common.Dtos;

namespace MoviesDb.Application.Movies.Dtos;

public record GetMoviesListRequest : PagedRequest
{
    public string? Q { get; set; }
    public int? YearOfRelease { get; set; }
}

