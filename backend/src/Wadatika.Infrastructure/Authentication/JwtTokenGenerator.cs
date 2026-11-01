using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wadatika.Application.Interfaces;

namespace Wadatika.Infrastructure.Authentication;

public sealed class JwtTokenGenerator(
    IConfiguration configuration)
    : IJwtTokenGenerator
{
    public LoginToken GenerateToken(Guid userId, string email)
    {
        var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT secret key is not configured.");

        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        var tokenMinutes = configuration.GetValue<int>(
            "Jwt:AccessTokenMinutes");

        if (tokenMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT access token lifetime must be greater than zero.");
        }

        var key = new SymmetricSecurityKey(
            Convert.FromBase64String(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(tokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new LoginToken(accessToken, expiresAt);
    }
}