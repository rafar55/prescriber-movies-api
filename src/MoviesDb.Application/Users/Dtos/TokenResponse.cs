using MoviesDb.Domain.Models;

namespace MoviesDb.Application.Users.Dtos;

public record TokenResponse(string AccessToken, double ExperiesInSeconds, string TokenType);

