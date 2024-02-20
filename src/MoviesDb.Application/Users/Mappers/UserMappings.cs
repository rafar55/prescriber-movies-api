using MoviesDb.Application.Users.Dtos;
using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Users.Mappers;

internal static class UserMappings
{
    public static UserResponse MapToResponse(this User user)
    {
        return new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, user.CreatedAt, user.IsAdmin);       
    }

    public static User MapToModel(this CreateUserRequest request)
    {
        return new User
        {            
            Id = Guid.NewGuid(),            
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName        
        };
    }
}
