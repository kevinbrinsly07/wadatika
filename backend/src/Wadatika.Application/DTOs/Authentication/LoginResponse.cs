namespace Wadatika.Application.DTOs.Authentication;

public sealed record LoginResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);