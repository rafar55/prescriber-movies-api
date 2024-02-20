using BCryptHasher = BCrypt.Net.BCrypt;
using MoviesDb.Application.Common.Interfaces;

namespace MoviesDb.Infrastructure.Authentication;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCryptHasher.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCryptHasher.Verify(password, hash);
    }
}
