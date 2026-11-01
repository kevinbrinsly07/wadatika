using System.Security.Cryptography;
using System.Text;

namespace Wadatika.Infrastructure.Authentication;

public sealed class RefreshTokenGenerator
{
    public string GenerateToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    public string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);

        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}