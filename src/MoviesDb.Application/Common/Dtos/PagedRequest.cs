namespace MoviesDb.Application.Common.Dtos;

public abstract record PagedRequest
{
    public int PageSize { get; set; } = 10;
    public int Page { get; set; } = 1;
    public int PageOffset => (Page - 1) * PageSize;
};

