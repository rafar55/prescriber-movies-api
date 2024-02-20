using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<int> CreateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
}
