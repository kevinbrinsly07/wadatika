using Wadatika.Application.DTOs.Projects;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;

namespace Wadatika.Application.Features.Projects;

public sealed class ProjectService(
    IProjectRepository projectRepository)
    : IProjectService
{
    public async Task<ProjectResponse> CreateAsync(
        Guid organizationId,
        CreateProjectRequest request,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var isMember = await projectRepository.IsOrganizationMemberAsync(
            organizationId,
            userId,
            cancellationToken);

        if (!isMember)
        {
            throw new OrganizationAccessDeniedException();
        }

        var normalizedKey = request.Key.Trim().ToUpperInvariant();

        var keyExists = await projectRepository.KeyExistsAsync(
            organizationId,
            normalizedKey,
            cancellationToken);

        if (keyExists)
        {
            throw new ProjectKeyAlreadyExistsException(normalizedKey);
        }

        var project = new Project(
            organizationId,
            request.Name.Trim(),
            normalizedKey,
            request.Description?.Trim());

        await projectRepository.AddAsync(
            project,
            cancellationToken);

        return MapToResponse(project);
    }

    public async Task<IReadOnlyList<ProjectResponse>?> GetByOrganizationAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var isMember = await projectRepository.IsOrganizationMemberAsync(
            organizationId,
            userId,
            cancellationToken);

        if (!isMember)
        {
            return null;
        }

        var projects = await projectRepository.GetByOrganizationAsync(
            organizationId,
            cancellationToken);

        return projects
            .Select(MapToResponse)
            .ToList();
    }

    private static ProjectResponse MapToResponse(Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.OrganizationId,
            project.Name,
            project.Key,
            project.Description,
            project.CreatedAt);
    }
}