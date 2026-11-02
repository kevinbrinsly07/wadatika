namespace Wadatika.Domain.Entities;

public sealed class Project
{
    private Project()
    {
    }

    public Project(
        Guid organizationId,
        string name,
        string key,
        string? description = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Name = ValidateName(name);
        Key = ValidateKey(key);
        Description = ValidateDescription(description);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Key { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void Rename(string newName)
    {
        Name = ValidateName(newName);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = ValidateDescription(description);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Project name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 100)
        {
            throw new ArgumentException(
                "Project name cannot exceed 100 characters.",
                nameof(name));
        }

        return normalizedName;
    }

    private static string ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(
                "Project key is required.",
                nameof(key));
        }

        var normalizedKey = key.Trim().ToUpperInvariant();

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                normalizedKey,
                @"^[A-Z][A-Z0-9]{1,9}$"))
        {
            throw new ArgumentException(
                "Project key must start with a letter and contain 2–10 uppercase letters or numbers.",
                nameof(key));
        }

        return normalizedKey;
    }

    private static string? ValidateDescription(string? description)
    {
        if (description is null)
        {
            return null;
        }

        var normalizedDescription = description.Trim();

        if (normalizedDescription.Length > 500)
        {
            throw new ArgumentException(
                "Project description cannot exceed 500 characters.",
                nameof(description));
        }

        return string.IsNullOrWhiteSpace(normalizedDescription)
            ? null
            : normalizedDescription;
    }
}