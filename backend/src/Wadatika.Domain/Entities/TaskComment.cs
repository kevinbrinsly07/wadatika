namespace Wadatika.Domain.Entities;

public sealed class TaskComment
{
    private TaskComment()
    {
    }

    public TaskComment(
        Guid projectTaskId,
        Guid authorUserId,
        string content)
    {
        if (projectTaskId == Guid.Empty)
            throw new ArgumentException(
                "Task ID is required.",
                nameof(projectTaskId));

        if (authorUserId == Guid.Empty)
            throw new ArgumentException(
                "Author ID is required.",
                nameof(authorUserId));

        Id = Guid.NewGuid();
        ProjectTaskId = projectTaskId;
        AuthorUserId = authorUserId;
        Content = ValidateContent(content);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ProjectTaskId { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void Edit(string content)
    {
        Content = ValidateContent(content);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string ValidateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "Comment content is required.",
                nameof(content));

        var normalized = content.Trim();

        if (normalized.Length > 5000)
            throw new ArgumentException(
                "Comment cannot exceed 5000 characters.",
                nameof(content));

        return normalized;
    }
}