using MoviesDb.Application.Users.Dtos;

namespace MoviesDb.Application.Users.Services;

public interface IUserService
{
    Task<TokenResponse> AuthenticateAsync(LoginRequest loginRequest);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse> GetByEmailAsync(string email);
}
