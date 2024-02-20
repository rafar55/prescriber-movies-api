using System.Text.Json.Serialization;

namespace MoviesDb.Application.Common.Dtos;

public abstract class PagedRequest
{
    public int PageSize { get; set; } = 10;
    public int Page { get; set; } = 1;

  
    public int GetOffset() => (Page - 1) * PageSize;
};

