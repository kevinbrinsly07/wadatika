namespace Wadatika.Application.DTOs.Organizations;

public sealed record OrganizationResponse(
     Guid Id,
     string Name,
     string Slug,
     DateTimeOffset CreatedAt
);