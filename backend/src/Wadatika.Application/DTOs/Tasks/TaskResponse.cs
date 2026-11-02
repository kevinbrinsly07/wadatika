using WorkStatus = Wadatika.Domain.Enums.TaskStatus;
using Wadatika.Domain.Enums;

namespace Wadatika.Application.DTOs.Tasks;

public sealed record TaskResponse(
    Guid Id,
    Guid ProjectId,
    Guid CreatedByUserId,
    Guid? AssigneeUserId,
    string Title,
    string? Description,
    WorkStatus Status,
    TaskPriority Priority,
    DateOnly? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);