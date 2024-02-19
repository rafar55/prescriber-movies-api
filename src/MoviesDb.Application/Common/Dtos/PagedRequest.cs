namespace MoviesDb.Application.Common.Dtos;

public abstract record PagedRequest(int page = 1, int pageSize = 10);

