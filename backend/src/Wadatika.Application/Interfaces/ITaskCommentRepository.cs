using Wadatika.Domain.Entities;

namespace Wadatika.Application.Interfaces;

public interface ITaskCommentRepository
{
    Task<bool> IsTaskAccessibleAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        TaskComment comment,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskComment>> GetByTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken);
}