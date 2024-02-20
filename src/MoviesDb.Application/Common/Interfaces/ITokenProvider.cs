using MoviesDb.Application.Users.Dtos;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Common.Interfaces;

public interface ITokenProvider
{
    TokenResponse GenerateToken(User user);
}
