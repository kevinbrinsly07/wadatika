using Wadatika.Application.DTOs.Projects;

namespace Wadatika.Application.Features.Projects;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(
        Guid organizationId,
        CreateProjectRequest request,
        Guid userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectResponse>?> GetByOrganizationAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken);
}