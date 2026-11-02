
using Wadatika.Application.DTOs.Organizations;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;
using Wadatika.Domain.Enums;

namespace Wadatika.Application.Features.Organizations;

public sealed class OrganizationService(
    IOrganizationRepository organizationRepository)
    : IOrganizationService
{
    public async Task<OrganizationResponse> CreateAsync(
        CreateOrganizationRequest request,
        Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var slugExists = await organizationRepository.SlugExistsAsync(
            normalizedSlug,
            cancellationToken);

        if (slugExists)
        {
            throw new OrganizationSlugAlreadyExistsException(normalizedSlug);
        }

        var organization = new Organization(
            request.Name,
            normalizedSlug);

        var ownerMembership = new OrganizationMember(
            organization.Id,
            ownerUserId,
            OrganizationRole.Owner);

        await organizationRepository.AddAsync(
            organization,
            ownerMembership,
            cancellationToken);

        return MapToResponse(organization);
    }

    public async Task<OrganizationResponse?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdForMemberAsync(
            id,
            userId,
            cancellationToken);

        return organization is null
            ? null
            : MapToResponse(organization);
    }

    private static OrganizationResponse MapToResponse(
        Organization organization)
    {
        return new OrganizationResponse(
            organization.Id,
            organization.Name,
            organization.Slug,
            organization.CreatedAt);
    }

    public async Task<IReadOnlyList<OrganizationResponse>> GetAllForUserAsync(
    Guid userId,
    CancellationToken cancellationToken)
    {
        var organizations = await organizationRepository.GetAllForMemberAsync(
            userId,
            cancellationToken);

        return organizations
            .Select(MapToResponse)
            .ToArray();
    }
}
