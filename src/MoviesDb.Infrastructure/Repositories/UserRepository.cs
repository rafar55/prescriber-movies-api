using Microsoft.Data.SqlClient;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Domain.Models;
using MoviesDb.Infrastructure.Database;
using System.Data;

namespace MoviesDb.Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<int> CreateAsync(User user)
    {
        using var cnn = await _dbConnectionFactory.CreateConnectionAsync();
        var sql = """
            INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, CreatedAt, IsAdmin)
            VALUES (@Id, @Email, @PasswordHash, @FirstName, @LastName, @CreatedAt, @IsAdmin)
            """;
        using var command = new SqlCommand(sql, (SqlConnection) cnn);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@FirstName", user.FirstName);
        command.Parameters.AddWithValue("@LastName", user.LastName);
        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
        command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var cnn = await _dbConnectionFactory.CreateConnectionAsync();
        var sql = """
            SELECT Id, Email, PasswordHash, FirstName, LastName, CreatedAt, IsAdmin
            FROM Users
            WHERE Email = @Email
            """;
        using var command = new SqlCommand(sql, (SqlConnection)cnn);
        using var reader = command.ExecuteReader(CommandBehavior.SingleRow);
        command.Parameters.AddWithValue("@Email", email);
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            LastName = reader.GetString(reader.GetOrdinal("LastName")),
            CreatedAt = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedAt")),
            IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
        };

    }
}
