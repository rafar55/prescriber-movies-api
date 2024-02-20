namespace MoviesDb.Application.Users.Dtos;

public record UserResponse(Guid Id, string Email, string FirstName, string LastName, DateTimeOffset CreatedAt, bool isAdmin);

