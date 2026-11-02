using Wadatika.Application.DTOs.Tasks;

namespace Wadatika.Application.Features.Tasks;

public interface IProjectTaskService
{
    Task<TaskResponse> CreateAsync(
        Guid organizationId,
        Guid projectId,
        CreateTaskRequest request,
        Guid userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskResponse>?> GetByProjectAsync(
        Guid organizationId,
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<TaskResponse> UpdateStatusAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        UpdateTaskStatusRequest request,
        Guid userId,
        CancellationToken cancellationToken);

    Task<TaskResponse> UpdatePriorityAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        UpdateTaskPriorityRequest request,
        Guid userId,
        CancellationToken cancellationToken);

    Task<TaskResponse> AssignAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        AssignTaskRequest request,
        Guid userId,
        CancellationToken cancellationToken);
}