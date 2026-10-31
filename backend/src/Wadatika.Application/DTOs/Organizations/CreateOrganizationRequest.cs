namespace Wadatika.Application.DTOs.Organizations;

public sealed record CreateOrganizationRequest(
     string Name,
     string Slug
);