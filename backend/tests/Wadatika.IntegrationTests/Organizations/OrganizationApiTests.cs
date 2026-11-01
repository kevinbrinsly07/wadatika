using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wadatika.Application.DTOs.Authentication;
using Wadatika.Application.DTOs.Organizations;
using Wadatika.Domain.Enums;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.UnitTests.Integration;

public sealed class OrganizationApiTests : IDisposable
{
    private readonly WadatikaApiFactory _factory;
    private readonly HttpClient _client;

    public OrganizationApiTests()
    {
        _factory = new WadatikaApiFactory();
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        dbContext.Database.Migrate();
    }

    [Fact]
    public async Task CreateOrganization_ShouldPersistAndReturnOrganization()
    {
        // Arrange: register a new user.
        var email = $"test-{Guid.NewGuid():N}@example.com";
        var password = "TestPassword123!";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/authentication/register",
            new RegisterRequest(email, password, password));

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var user = await registerResponse.Content
            .ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Log in to obtain an access token.
        var loginResponse = await _client.PostAsJsonAsync(
            "/api/authentication/login",
            new LoginRequest(email, password));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var login = await loginResponse.Content
            .ReadFromJsonAsync<LoginApiResponse>();

        Assert.NotNull(login);

        var ownerAccessToken = login.AccessToken;

        // Authenticate subsequent requests.
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        // Prepare organization request.
        var slug = $"test-{Guid.NewGuid():N}";

        var request = new CreateOrganizationRequest(
            "Integration Test Organization",
            slug);

        // Act: create the organization.
        var response = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        // Assert: organization was created.
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var organization = await response.Content
            .ReadFromJsonAsync<OrganizationResponse>();

        Assert.NotNull(organization);
        Assert.Equal("Integration Test Organization", organization.Name);
        Assert.Equal(slug, organization.Slug);

        // Verify organization and owner membership in PostgreSQL.
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var savedOrganization = await dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Slug == slug);

        Assert.NotNull(savedOrganization);
        Assert.Equal(organization.Id, savedOrganization.Id);

        var ownerMembership = await dbContext.OrganizationMember
            .AsNoTracking()
            .SingleOrDefaultAsync(member =>
                member.OrganizationId == organization.Id &&
                member.UserId == user.Id);

        Assert.NotNull(ownerMembership);
        Assert.Equal(OrganizationRole.Owner, ownerMembership.Role);

        // Verify duplicate slugs return HTTP 409.
        var duplicateResponse = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            duplicateResponse.StatusCode);

        // Verify organization retrieval.
        var getResponse = await _client.GetAsync(
            $"/api/organizations/{organization.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // An unauthenticated user must not access the organization.
        _client.DefaultRequestHeaders.Authorization = null;

        var anonymousResponse = await _client.GetAsync(
            $"/api/organizations/{organization.Id}");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            anonymousResponse.StatusCode);

        // Register a second user.
        var secondEmail = $"test-{Guid.NewGuid():N}@example.com";

        var secondRegisterResponse = await _client.PostAsJsonAsync(
            "/api/authentication/register",
            new RegisterRequest(
                secondEmail,
                password,
                password));

        Assert.Equal(
            HttpStatusCode.Created,
            secondRegisterResponse.StatusCode);

        // Log in as the second user.
        var secondLoginResponse = await _client.PostAsJsonAsync(
            "/api/authentication/login",
            new LoginRequest(secondEmail, password));

        Assert.Equal(
            HttpStatusCode.OK,
            secondLoginResponse.StatusCode);

        var secondLogin = await secondLoginResponse.Content
            .ReadFromJsonAsync<LoginApiResponse>();

        Assert.NotNull(secondLogin);

        // Authenticate as the second user.
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                secondLogin.AccessToken);

        // The second user is not a member of the first user's organization.
        var forbiddenResponse = await _client.GetAsync(
            $"/api/organizations/{organization.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            forbiddenResponse.StatusCode);

        // Restore the organization owner's access token.
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                ownerAccessToken);

        // The owner must still be able to access the organization.
        var ownerResponse = await _client.GetAsync(
            $"/api/organizations/{organization.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            ownerResponse.StatusCode);
    }

    private sealed record LoginApiResponse(
        Guid UserId,
        string Email,
        string AccessToken,
        DateTimeOffset ExpiresAt);

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}