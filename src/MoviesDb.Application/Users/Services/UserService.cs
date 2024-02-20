using FluentValidation;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Users.Dtos;
using MoviesDb.Application.Users.Mappers;
using MoviesDb.Domain.Exceptions;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly TimeProvider _timeProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public UserService(
        IUserRepository userRepository, 
        IValidator<CreateUserRequest> createValidator,
        IValidator<LoginRequest> loginValidator,
        TimeProvider timeProvider,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _createValidator = createValidator;
        _loginValidator = loginValidator;
        _timeProvider = timeProvider;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<TokenResponse> AuthenticateAsync(LoginRequest loginRequest)
    {
        await _loginValidator.ValidateAndThrowAsync(loginRequest);

        var dbUser = await _userRepository.GetByEmailAsync(loginRequest.Email);
        if (dbUser is null || !_passwordHasher.VerifyPassword(loginRequest.Password, dbUser.PasswordHash))
        {
            throw new AuthenticationFailedException();
        }

        return _tokenProvider.GenerateToken(dbUser);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var dbModel = request.MapToModel();
        dbModel.Id = Guid.NewGuid();
        dbModel.CreatedAt = _timeProvider.GetUtcNow();
        dbModel.PasswordHash = _passwordHasher.HashPassword(request.Password);

        await _userRepository.CreateAsync(dbModel);

        return dbModel.MapToResponse();
    }
}
