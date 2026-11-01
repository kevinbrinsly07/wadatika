using Wadatika.Application.DTOs.Authentication;

namespace Wadatika.Application.Features.Authentication;

public interface IAuthenticationService
{
    Task<UserResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default);
}