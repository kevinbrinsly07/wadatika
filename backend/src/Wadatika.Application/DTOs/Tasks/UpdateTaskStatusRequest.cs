using WorkStatus = Wadatika.Domain.Enums.TaskStatus;

namespace Wadatika.Application.DTOs.Tasks;

public sealed record UpdateTaskStatusRequest(WorkStatus Status);