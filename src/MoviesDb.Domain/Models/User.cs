namespace MoviesDb.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string PasswordHash { get; set; }
    public bool IsAdmin { get; set; }
}
