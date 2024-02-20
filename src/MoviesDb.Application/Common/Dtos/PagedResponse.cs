namespace MoviesDb.Application.Common.Dtos;

public record PagedResponse<TResponse> 
{
    public required int PageSize { get; init; }
    public required int Page { get; init; }
    public required int TotalItems { get; init; }
    public int NumberOfPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    public bool HasNextPage => TotalItems > (Page * PageSize);
    public bool HasPreviousPage => Page > 1;


    public required IEnumerable<TResponse> Items { get; init; } = Enumerable.Empty<TResponse>();

    public static PagedResponse<TResponse> Empty(int pageSize, int page) => new()
    {
        PageSize = pageSize,
        Page = page,
        TotalItems = 0,
        Items = Enumerable.Empty<TResponse>()
    };
}
