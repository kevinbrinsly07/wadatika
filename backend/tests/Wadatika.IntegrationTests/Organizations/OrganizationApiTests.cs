using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wadatika.Application.DTOs.Organizations;
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
        // Arrange
        var slug = $"test-{Guid.NewGuid():N}";

        var request = new CreateOrganizationRequest(
            "Integration Test Organization",
            slug);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var organization = await response.Content
            .ReadFromJsonAsync<OrganizationResponse>();

        Assert.NotNull(organization);
        Assert.Equal("Integration Test Organization", organization.Name);
        Assert.Equal(slug, organization.Slug);

        // Verify the organization was actually stored in PostgreSQL.
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var savedOrganization = await dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Slug == slug);

        Assert.NotNull(savedOrganization);
        Assert.Equal(organization.Id, savedOrganization.Id);

        // Verify duplicate slugs return HTTP 409.
        var duplicateResponse = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            duplicateResponse.StatusCode);

        // Verify the resource can be retrieved.
        var getResponse = await _client.GetAsync(
            $"/api/organizations/{organization.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}