namespace Wadatika.Application.DTOs.Projects;

public sealed record ProjectResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Key,
    string? Description,
    DateTimeOffset CreatedAt);