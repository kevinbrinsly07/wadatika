
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Wadatika.Application.DTOs.Authentication;

namespace Wadatika.UnitTests.Integration;

public sealed class AuthenticationApiTests : IDisposable
{
     private const string RefreshTokenCookieName = "refreshToken";

     private readonly WadatikaApiFactory _factory;
     private readonly HttpClient _client;

     public AuthenticationApiTests()
     {
          _factory = new WadatikaApiFactory();

          // We manage cookies ourselves so we can test token reuse.
          _client = _factory.CreateClient(
              new WebApplicationFactoryClientOptions
              {
                   HandleCookies = false
              });
     }

     [Fact]
     public async Task Refresh_ShouldRotateToken_AndRejectOldToken()
     {
          // Arrange: create a unique user for this test.
          var email = $"test-{Guid.NewGuid():N}@example.com";
          var password = "TestPassword123!";

          var registerRequest = new RegisterRequest(
              email,
              password,
              password);

          var registerResponse = await _client.PostAsJsonAsync(
              "/api/authentication/register",
              registerRequest);

          Assert.Equal(
              HttpStatusCode.Created,
              registerResponse.StatusCode);

          // Log in to obtain the initial refresh-token cookie.
          var loginResponse = await _client.PostAsJsonAsync(
              "/api/authentication/login",
              new LoginRequest(email, password));

          Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

          var originalRefreshToken = GetRefreshToken(loginResponse);

          // Act: exchange the original token for a replacement.
          var firstRefreshResponse = await RefreshAsync(
              originalRefreshToken);

          // Assert: the first refresh succeeds.
          Assert.Equal(
              HttpStatusCode.OK,
              firstRefreshResponse.StatusCode);

          var replacementRefreshToken = GetRefreshToken(
              firstRefreshResponse);

          Assert.NotEqual(
              originalRefreshToken,
              replacementRefreshToken);

          // Act: attempt to reuse the original token.
          var reuseResponse = await RefreshAsync(
              originalRefreshToken);

          // Assert: the original token cannot be reused.
          Assert.Equal(
              HttpStatusCode.Unauthorized,
              reuseResponse.StatusCode);

          // The replacement token should still be valid.
          var secondRefreshResponse = await RefreshAsync(
              replacementRefreshToken);

          Assert.Equal(
              HttpStatusCode.OK,
              secondRefreshResponse.StatusCode);
     }

     [Fact]
     public async Task Logout_ShouldRevokeRefreshToken_AndClearCookie()
     {
          // Arrange: create a unique user.
          var email = $"test-{Guid.NewGuid():N}@example.com";
          var password = "TestPassword123!";

          var registerResponse = await _client.PostAsJsonAsync(
              "/api/authentication/register",
              new RegisterRequest(email, password, password));

          Assert.Equal(
              HttpStatusCode.Created,
              registerResponse.StatusCode);

          // Log in to obtain a refresh token.
          var loginResponse = await _client.PostAsJsonAsync(
              "/api/authentication/login",
              new LoginRequest(email, password));

          Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

          var refreshToken = GetRefreshToken(loginResponse);

          // Act: log out using the refresh-token cookie.
          using var logoutRequest = new HttpRequestMessage(
              HttpMethod.Post,
              "/api/authentication/logout");

          logoutRequest.Headers.Add(
              "Cookie",
              $"{RefreshTokenCookieName}={refreshToken}");

          var logoutResponse = await _client.SendAsync(logoutRequest);

          // Assert: logout succeeds.
          Assert.Equal(
              HttpStatusCode.NoContent,
              logoutResponse.StatusCode);

          // Assert: the API sends a cookie-clearing header.
          Assert.True(
              logoutResponse.Headers.TryGetValues(
                  "Set-Cookie",
                  out var cookieHeaders));

          var clearedCookie = Assert.Single(cookieHeaders!);

          Assert.Contains(
              $"{RefreshTokenCookieName}=",
              clearedCookie);

          // Assert: the old refresh token can no longer be used.
          var refreshResponse = await RefreshAsync(refreshToken);

          Assert.Equal(
              HttpStatusCode.Unauthorized,
              refreshResponse.StatusCode);
     }

     private async Task<HttpResponseMessage> RefreshAsync(
         string refreshToken)
     {
          using var request = new HttpRequestMessage(
              HttpMethod.Post,
              "/api/authentication/refresh");

          request.Headers.Add(
              "Cookie",
              $"{RefreshTokenCookieName}={refreshToken}");

          return await _client.SendAsync(request);
     }

     private static string GetRefreshToken(
         HttpResponseMessage response)
     {
          Assert.True(
              response.Headers.TryGetValues(
                  "Set-Cookie",
                  out var cookieHeaders));

          var cookieHeader = Assert.Single(cookieHeaders!);

          var cookiePair = cookieHeader.Split(';', 2)[0];

          const string prefix = RefreshTokenCookieName + "=";

          Assert.StartsWith(prefix, cookiePair);

          return cookiePair[prefix.Length..];
     }

     public void Dispose()
     {
          _client.Dispose();
          _factory.Dispose();
     }
}
