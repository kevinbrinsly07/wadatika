using WorkPriority = Wadatika.Domain.Enums.TaskPriority;
using WorkStatus = Wadatika.Domain.Enums.TaskStatus;

namespace Wadatika.Domain.Entities;

public sealed class ProjectTask
{
    private ProjectTask()
    {
    }

    public ProjectTask(
        Guid projectId,
        Guid createdByUserId,
        string title,
        string? description = null,
        DateOnly? dueDate = null)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID is required.", nameof(projectId));

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Creator ID is required.", nameof(createdByUserId));

        Id = Guid.NewGuid();
        ProjectId = projectId;
        CreatedByUserId = createdByUserId;
        Title = ValidateTitle(title);
        Description = ValidateDescription(description);
        DueDate = dueDate;
        Status = WorkStatus.Todo;
        Priority = WorkPriority.Medium;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ProjectId { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public Guid? AssigneeUserId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public WorkStatus Status { get; private set; }

    public WorkPriority Priority { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void UpdateDetails(string title, string? description)
    {
        Title = ValidateTitle(title);
        Description = ValidateDescription(description);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeStatus(WorkStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangePriority(WorkPriority priority)
    {
        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        Priority = priority;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AssignTo(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        AssigneeUserId = userId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Unassign()
    {
        AssigneeUserId = null;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetDueDate(DateOnly? dueDate)
    {
        DueDate = dueDate;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title is required.", nameof(title));

        var normalized = title.Trim();

        if (normalized.Length > 200)
            throw new ArgumentException(
                "Task title cannot exceed 200 characters.",
                nameof(title));

        return normalized;
    }

    private static string? ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        var normalized = description.Trim();

        if (normalized.Length > 2000)
            throw new ArgumentException(
                "Task description cannot exceed 2000 characters.",
                nameof(description));

        return normalized;
    }
}