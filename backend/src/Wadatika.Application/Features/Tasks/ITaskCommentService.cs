using Wadatika.Application.DTOs.Tasks;

namespace Wadatika.Application.Features.Tasks;

public interface ITaskCommentService
{
    Task<TaskCommentResponse> CreateAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        CreateTaskCommentRequest request,
        Guid userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskCommentResponse>?> GetByTaskAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken);
}