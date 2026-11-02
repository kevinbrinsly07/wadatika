using Wadatika.Domain.Enums;

namespace Wadatika.Application.DTOs.Tasks;

public sealed record UpdateTaskPriorityRequest(TaskPriority Priority);