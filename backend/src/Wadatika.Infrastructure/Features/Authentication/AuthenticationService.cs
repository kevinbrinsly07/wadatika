using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wadatika.Application.DTOs.Authentication;
using Wadatika.Application.Features.Authentication;
using Wadatika.Application.Interfaces;
using Wadatika.Infrastructure.Authentication;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.Infrastructure.Features.Authentication;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    RefreshTokenGenerator refreshTokenGenerator,
    AppDbContext dbContext,
    IConfiguration configuration)
    : Wadatika.Application.Features.Authentication.IAuthenticationService
{
    public async Task<UserResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim();

        var existingUser = await userManager.FindByEmailAsync(normalizedEmail);

        if (existingUser is not null)
        {
            throw new EmailAlreadyRegisteredException(normalizedEmail);
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var duplicateEmail = result.Errors.Any(error =>
                error.Code == "DuplicateEmail" ||
                error.Code == "DuplicateUserName");

            if (duplicateEmail)
            {
                throw new EmailAlreadyRegisteredException(normalizedEmail);
            }

            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"User registration failed: {errors}");
        }

        return new UserResponse(user.Id, user.Email!);
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim();
        var user = await userManager.FindByEmailAsync(normalizedEmail);

        if (user is null ||
            !await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new InvalidCredentialsException();
        }

        var loginToken = jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email!);

        var refreshToken = refreshTokenGenerator.GenerateToken();
        var refreshTokenHash = refreshTokenGenerator.HashToken(refreshToken);
        var refreshTokenExpiresAt = GetRefreshTokenExpiresAt();

        await dbContext.RefreshTokens.AddAsync(
            new RefreshToken(user.Id, refreshTokenHash, refreshTokenExpiresAt),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            user.Id,
            user.Email!,
            loginToken.AccessToken,
            loginToken.ExpiresAt,
            refreshToken,
            refreshTokenExpiresAt);
    }

    public async Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var tokenHash = refreshTokenGenerator.HashToken(request.RefreshToken);

        var storedToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            throw new InvalidCredentialsException();
        }

        var user = await userManager.FindByIdAsync(
            storedToken.UserId.ToString());

        if (user is null || user.Email is null)
        {
            throw new InvalidCredentialsException();
        }

        var replacementRefreshToken = refreshTokenGenerator.GenerateToken();
        var replacementHash = refreshTokenGenerator.HashToken(
            replacementRefreshToken);
        var replacementExpiresAt = GetRefreshTokenExpiresAt();
        var now = DateTimeOffset.UtcNow;

        // Atomically claim this refresh token. Only one concurrent request
        // can change an unrevoked, unexpired token.
        var updatedRows = await dbContext.RefreshTokens
            .Where(token =>
                token.Id == storedToken.Id &&
                token.RevokedAt == null &&
                token.ExpiresAt > now)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(token => token.RevokedAt, now)
                    .SetProperty(token => token.ReplacedByTokenHash, replacementHash),
                cancellationToken);

        if (updatedRows != 1)
        {
            throw new InvalidCredentialsException();
        }

        await dbContext.RefreshTokens.AddAsync(
            new RefreshToken(
                user.Id,
                replacementHash,
                replacementExpiresAt),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var loginToken = jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email);

        return new LoginResponse(
            user.Id,
            user.Email,
            loginToken.AccessToken,
            loginToken.ExpiresAt,
            replacementRefreshToken,
            replacementExpiresAt);
    }

    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = refreshTokenGenerator.HashToken(refreshToken);
        var now = DateTimeOffset.UtcNow;

        // Idempotent: an unknown or already-revoked token changes no rows.
        await dbContext.RefreshTokens
            .Where(token =>
                token.TokenHash == tokenHash &&
                token.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    token => token.RevokedAt,
                    now),
                cancellationToken);
    }

    private DateTimeOffset GetRefreshTokenExpiresAt()
    {
        var refreshTokenDays = configuration.GetValue<int>(
            "Jwt:RefreshTokenDays");

        if (refreshTokenDays <= 0)
        {
            throw new InvalidOperationException(
                "JWT refresh token lifetime must be greater than zero.");
        }

        return DateTimeOffset.UtcNow.AddDays(refreshTokenDays);
    }
}