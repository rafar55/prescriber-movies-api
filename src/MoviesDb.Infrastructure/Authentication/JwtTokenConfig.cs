namespace MoviesDb.Infrastructure.Authentication;

public class JwtTokenConfig
{
    public const string SectionName = "JwtTokenConfig";

    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int ExpirationInMinutes { get; init; }
}
