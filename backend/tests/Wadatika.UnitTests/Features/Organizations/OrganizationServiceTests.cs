using Wadatika.Application.DTOs.Organizations;
using Wadatika.Application.Features.Organizations;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;

namespace Wadatika.UnitTests.Features.Organizations;

public class OrganizationServiceTests
{
     [Fact]
     public async Task CreateAsync_ShouldCreateOrganization_WhenSlugIsAvailable()
     {
          // Arrange
          var repository = new FakeOrganizationRepository();
          var service = new OrganizationService(repository);

          var request = new CreateOrganizationRequest(
              "ABC Technologies",
              "abc-technologies");

          // Act
          var result = await service.CreateAsync(
              request,
              CancellationToken.None);

          // Assert
          Assert.NotEqual(Guid.Empty, result.Id);
          Assert.Equal("ABC Technologies", result.Name);
          Assert.Equal("abc-technologies", result.Slug);
          Assert.Single(repository.Organizations);
     }

     [Fact]
     public async Task CreateAsync_ShouldThrowException_WhenSlugAlreadyExists()
     {
          // Arrange
          var repository = new FakeOrganizationRepository();
          repository.Organizations.Add(
              new Organization("ABC Technologies", "abc-technologies"));

          var service = new OrganizationService(repository);

          var request = new CreateOrganizationRequest(
              "Another Company",
              "abc-technologies");

          // Act & Assert
          await Assert.ThrowsAsync<OrganizationSlugAlreadyExistsException>(
              () => service.CreateAsync(request, CancellationToken.None));

          Assert.Single(repository.Organizations);
     }

     private sealed class FakeOrganizationRepository
         : IOrganizationRepository
     {
          public List<Organization> Organizations { get; } = [];

          public Task<bool> SlugExistsAsync(
              string slug,
              CancellationToken cancellationToken)
          {
               return Task.FromResult(
                   Organizations.Any(organization =>
                       organization.Slug == slug));
          }

          public Task AddAsync(
              Organization organization,
              CancellationToken cancellationToken)
          {
               Organizations.Add(organization);
               return Task.CompletedTask;
          }

          public Task<Organization?> GetByIdAsync(
              Guid id,
              CancellationToken cancellationToken)
          {
               return Task.FromResult(
                   Organizations.FirstOrDefault(
                       organization => organization.Id == id));
          }
     }
}