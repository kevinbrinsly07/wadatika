using Wadatika.Domain.Entities;

namespace Wadatika.Application.Interfaces;

public interface IProjectTaskRepository
{
    Task<bool> IsProjectAccessibleAsync(
        Guid organizationId,
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> IsProjectMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        ProjectTask task,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectTask>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken);

    Task<ProjectTask?> GetByIdAsync(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}