namespace Wadatika.Application.DTOs.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    DateOnly? DueDate);