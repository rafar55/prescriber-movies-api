using Microsoft.IdentityModel.Tokens;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Users.Dtos;
using MoviesDb.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesDb.Infrastructure.Authentication;

public class JwtTokenProvider : ITokenProvider
{
    private readonly JwtTokenConfig _jwtTokenConfig;

    public JwtTokenProvider(JwtTokenConfig jwtTokenConfig)
    {
        _jwtTokenConfig = jwtTokenConfig;
    }

    public TokenResponse GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtTokenConfig.SecretKey);       
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtTokenConfig.ExpirationInMinutes),
            Issuer = _jwtTokenConfig.Issuer,
            Audience = _jwtTokenConfig.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var expiresIn = TimeSpan.FromMinutes(_jwtTokenConfig.ExpirationInMinutes);
        return new TokenResponse(tokenHandler.WriteToken(token), expiresIn.TotalSeconds, "Bearer");
    }
}
