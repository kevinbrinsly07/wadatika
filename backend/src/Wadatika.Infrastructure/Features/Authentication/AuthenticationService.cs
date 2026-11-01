using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wadatika.Application.Interfaces;
using Wadatika.Application.DTOs.Authentication;
using Wadatika.Application.Features.Authentication;
using Wadatika.Infrastructure.Authentication;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.Infrastructure.Features.Authentication;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    RefreshTokenGenerator refreshTokenGenerator,
    AppDbContext dbContext,
    IConfiguration configuration)
    : IAuthenticationService
{
     public async Task<UserResponse> RegisterAsync(
         RegisterRequest request,
         CancellationToken cancellationToken = default)
     {
          var normalizedEmail = request.Email.Trim();

          var existingUser = await userManager.FindByEmailAsync(
              normalizedEmail);

          if (existingUser is not null)
          {
               throw new EmailAlreadyRegisteredException(normalizedEmail);
          }

          var user = new ApplicationUser
          {
               UserName = normalizedEmail,
               Email = normalizedEmail
          };

          var result = await userManager.CreateAsync(
              user,
              request.Password);

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

          return new UserResponse(
              user.Id,
              user.Email!);
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
          var refreshTokenEntity = new RefreshToken(
              user.Id,
              refreshTokenHash,
              refreshTokenExpiresAt);

          await dbContext.RefreshTokens.AddAsync(
              refreshTokenEntity,
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

          var loginToken = jwtTokenGenerator.GenerateToken(
              user.Id,
              user.Email);
          var replacementRefreshToken = refreshTokenGenerator.GenerateToken();
          var replacementHash = refreshTokenGenerator.HashToken(
              replacementRefreshToken);
          var replacementExpiresAt = GetRefreshTokenExpiresAt();

          storedToken.Revoke(replacementHash);
          await dbContext.RefreshTokens.AddAsync(
              new RefreshToken(
                  user.Id,
                  replacementHash,
                  replacementExpiresAt),
              cancellationToken);
          await dbContext.SaveChangesAsync(cancellationToken);

          return new LoginResponse(
              user.Id,
              user.Email,
              loginToken.AccessToken,
              loginToken.ExpiresAt,
              replacementRefreshToken,
              replacementExpiresAt);
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