namespace MoviesDb.Application.Users.Dtos;

public record CreateUserRequest(
    string Email, 
    string FirstName, 
    string LastName, 
    string Password, 
    string ConfirmPassword,
    bool IsAdmin);

