namespace Wadatika.Application.DTOs.Projects;

public sealed record CreateProjectRequest(
    string Name,
    string Key,
    string? Description);