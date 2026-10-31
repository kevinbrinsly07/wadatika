using Wadatika.Application.DTOs.Organizations;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;

namespace Wadatika.Application.Features.Organizations;

public sealed class OrganizationService(
    IOrganizationRepository organizationRepository)
    : IOrganizationService
{
    public async Task<OrganizationResponse> CreateAsync(
        CreateOrganizationRequest request,
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

        await organizationRepository.AddAsync(
            organization,
            cancellationToken);

        return MapToResponse(organization);
    }

    public async Task<OrganizationResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(
            id,
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
}