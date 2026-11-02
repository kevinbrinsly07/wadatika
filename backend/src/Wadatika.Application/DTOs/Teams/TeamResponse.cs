namespace Wadatika.Application.DTOs.Teams;

public sealed record TeamResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    DateTimeOffset CreatedAt);