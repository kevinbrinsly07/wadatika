using Wadatika.Application.DTOs.Tasks;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;

namespace Wadatika.Application.Features.Tasks;

public sealed class ProjectTaskService(
    IProjectTaskRepository taskRepository)
    : IProjectTaskService
{
    public async Task<TaskResponse> CreateAsync(
        Guid organizationId,
        Guid projectId,
        CreateTaskRequest request,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var hasAccess = await taskRepository.IsProjectAccessibleAsync(
            organizationId,
            projectId,
            userId,
            cancellationToken);

        if (!hasAccess)
        {
            throw new TaskProjectAccessDeniedException();
        }

        var task = new ProjectTask(
            projectId,
            userId,
            request.Title,
            request.Description,
            request.DueDate);

        await taskRepository.AddAsync(task, cancellationToken);

        return MapToResponse(task);
    }

    public async Task<IReadOnlyList<TaskResponse>?> GetByProjectAsync(
        Guid organizationId,
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var hasAccess = await taskRepository.IsProjectAccessibleAsync(
            organizationId,
            projectId,
            userId,
            cancellationToken);

        if (!hasAccess)
        {
            return null;
        }

        var tasks = await taskRepository.GetByProjectAsync(
            projectId,
            cancellationToken);

        return tasks
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TaskResponse> UpdateStatusAsync(
    Guid organizationId,
    Guid projectId,
    Guid taskId,
    UpdateTaskStatusRequest request,
    Guid userId,
    CancellationToken cancellationToken)
    {
        var task = await GetAccessibleTaskAsync(
            organizationId,
            projectId,
            taskId,
            userId,
            cancellationToken);

        task.ChangeStatus(request.Status);

        await taskRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(task);
    }

    public async Task<TaskResponse> UpdatePriorityAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        UpdateTaskPriorityRequest request,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var task = await GetAccessibleTaskAsync(
            organizationId,
            projectId,
            taskId,
            userId,
            cancellationToken);

        task.ChangePriority(request.Priority);

        await taskRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(task);
    }

    public async Task<TaskResponse> AssignAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        AssignTaskRequest request,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var task = await GetAccessibleTaskAsync(
            organizationId,
            projectId,
            taskId,
            userId,
            cancellationToken);

        if (request.AssigneeUserId.HasValue)
        {
            var isMember = await taskRepository.IsProjectMemberAsync(
                projectId,
                request.AssigneeUserId.Value,
                cancellationToken);

            if (!isMember)
            {
                throw new TaskAssigneeNotFoundException();
            }

            task.AssignTo(request.AssigneeUserId.Value);
        }
        else
        {
            task.Unassign();
        }

        await taskRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(task);
    }

    private async Task<ProjectTask> GetAccessibleTaskAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var hasAccess = await taskRepository.IsProjectAccessibleAsync(
            organizationId,
            projectId,
            userId,
            cancellationToken);

        if (!hasAccess)
        {
            throw new TaskProjectAccessDeniedException();
        }

        var task = await taskRepository.GetByIdAsync(
            projectId,
            taskId,
            cancellationToken);

        return task ?? throw new TaskNotFoundException();
    }

    private static TaskResponse MapToResponse(ProjectTask task)
    {
        return new TaskResponse(
            task.Id,
            task.ProjectId,
            task.CreatedByUserId,
            task.AssigneeUserId,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt);
    }
}