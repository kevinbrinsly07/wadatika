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

        var ownerUserId = Guid.NewGuid();

        // Act
        var result = await service.CreateAsync(
            request,
            ownerUserId,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("ABC Technologies", result.Name);
        Assert.Equal("abc-technologies", result.Slug);

        Assert.Single(repository.Organizations);
        Assert.Single(repository.OrganizationMembers);

        var owner = Assert.Single(repository.OrganizationMembers);

        Assert.Equal(ownerUserId, owner.UserId);
        Assert.Equal(result.Id, owner.OrganizationId);
        Assert.Equal(
            Wadatika.Domain.Enums.OrganizationRole.Owner,
            owner.Role);
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
            () => service.CreateAsync(
                request,
                Guid.NewGuid(),
                CancellationToken.None));

        Assert.Single(repository.Organizations);
        Assert.Empty(repository.OrganizationMembers);
    }

    private sealed class FakeOrganizationRepository
        : IOrganizationRepository
    {
        public List<Organization> Organizations { get; } = [];

        public List<OrganizationMember> OrganizationMembers { get; } = [];

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
            OrganizationMember owner,
            CancellationToken cancellationToken)
        {
            Organizations.Add(organization);
            OrganizationMembers.Add(owner);

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

        public Task<Organization?> GetByIdForMemberAsync(
            Guid id,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var organization = Organizations.FirstOrDefault(item =>
                item.Id == id &&
                OrganizationMembers.Any(member =>
                    member.OrganizationId == id &&
                    member.UserId == userId));

            return Task.FromResult(organization);
        }
    }
}