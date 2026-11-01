
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wadatika.Application.DTOs.Authentication;
using Wadatika.Application.Features.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Wadatika.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthenticationController(
    IAuthenticationService authenticationService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator,
    IValidator<RefreshTokenRequest> refreshTokenValidator,
    IWebHostEnvironment environment)
    : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await registerValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    validationResult.ToDictionary()));
        }

        var user = await authenticationService.RegisterAsync(
            request,
            cancellationToken);

        return Created(
            $"/api/users/{user.Id}",
            user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await loginValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    validationResult.ToDictionary()));
        }

        var response = await authenticationService.LoginAsync(
            request,
            cancellationToken);

        SetRefreshTokenCookie(
            response.RefreshToken,
            response.RefreshTokenExpiresAt);

        return Ok(new
        {
            response.UserId,
            response.Email,
            response.AccessToken,
            response.ExpiresAt
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized();
        }

        var request = new RefreshTokenRequest(refreshToken);

        var validationResult =
            await refreshTokenValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    validationResult.ToDictionary()));
        }

        var response = await authenticationService.RefreshAsync(
            request,
            cancellationToken);

        SetRefreshTokenCookie(
            response.RefreshToken,
            response.RefreshTokenExpiresAt);

        return Ok(new
        {
            response.UserId,
            response.Email,
            response.AccessToken,
            response.ExpiresAt
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        if (userId is null || email is null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            UserId = userId,
            Email = email
        });
    }

    private void SetRefreshTokenCookie(
        string refreshToken,
        DateTimeOffset expiresAt)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !environment.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt,
                Path = "/api/authentication",
                IsEssential = true
            });
    }
}
