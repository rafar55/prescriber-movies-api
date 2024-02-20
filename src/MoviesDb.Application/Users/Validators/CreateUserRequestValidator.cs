using FluentValidation;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Users.Dtos;

namespace MoviesDb.Application.Users.Validators;

public class CreateUserRequestValidator: AbstractValidator<CreateUserRequest>
{
    private readonly IUserRepository _userRepository;

    public CreateUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(350)
            .MustAsync(ValidateEmail)
            .WithMessage("Email already exists");

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Password).MaximumLength(50).NotEmpty();
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
    }

    private async Task<bool> ValidateEmail(string email, CancellationToken token)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user is null;
    }
}
