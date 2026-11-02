using Wadatika.Application.DTOs.Tasks;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;

namespace Wadatika.Application.Features.Tasks;

public sealed class TaskCommentService(
    ITaskCommentRepository commentRepository)
    : ITaskCommentService
{
    public async Task<TaskCommentResponse> CreateAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        CreateTaskCommentRequest request,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var hasAccess = await commentRepository.IsTaskAccessibleAsync(
            organizationId,
            projectId,
            taskId,
            userId,
            cancellationToken);

        if (!hasAccess)
            throw new TaskProjectAccessDeniedException();

        var comment = new TaskComment(
            taskId,
            userId,
            request.Content);

        await commentRepository.AddAsync(comment, cancellationToken);

        return MapToResponse(comment);
    }

    public async Task<IReadOnlyList<TaskCommentResponse>?> GetByTaskAsync(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var hasAccess = await commentRepository.IsTaskAccessibleAsync(
            organizationId,
            projectId,
            taskId,
            userId,
            cancellationToken);

        if (!hasAccess)
            return null;

        var comments = await commentRepository.GetByTaskAsync(
            taskId,
            cancellationToken);

        return comments.Select(MapToResponse).ToList();
    }

    private static TaskCommentResponse MapToResponse(TaskComment comment)
    {
        return new TaskCommentResponse(
            comment.Id,
            comment.ProjectTaskId,
            comment.AuthorUserId,
            comment.Content,
            comment.CreatedAt,
            comment.UpdatedAt);
    }
}