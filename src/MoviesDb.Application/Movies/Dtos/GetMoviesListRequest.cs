using MoviesDb.Application.Common.Dtos;

namespace MoviesDb.Application.Movies.Dtos;

public class GetMoviesListRequest : PagedRequest
{
    public string? Title { get; set; }
    public int? YearOfRelease { get; set; }
}

