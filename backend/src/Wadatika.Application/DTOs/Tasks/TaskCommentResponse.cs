namespace Wadatika.Application.DTOs.Tasks;

public sealed record TaskCommentResponse(
    Guid Id,
    Guid ProjectTaskId,
    Guid AuthorUserId,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);