namespace Wadatika.Application.Interfaces;

public interface IJwtTokenGenerator
{
    LoginToken GenerateToken(Guid userId, string email);
}

public sealed record LoginToken(
    string AccessToken,
    DateTimeOffset ExpiresAt);