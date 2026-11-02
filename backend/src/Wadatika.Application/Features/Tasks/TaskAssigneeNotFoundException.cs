namespace Wadatika.Application.Features.Tasks;

public sealed class TaskAssigneeNotFoundException()
    : Exception("The assignee is not a member of this organization.");