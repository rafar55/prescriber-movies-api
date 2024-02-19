namespace MoviesDb.Domain.Models;

public class User
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string HashedPassword { get; set; }
    public bool IsAdmin { get; set; }
}
