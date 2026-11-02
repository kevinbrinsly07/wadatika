using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wadatika.Application.DTOs.Tasks;
using Wadatika.Application.Features.Tasks;


namespace Wadatika.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/projects/{projectId:guid}/tasks")]
public sealed class ProjectTasksController(
    IProjectTaskService taskService,
    IValidator<CreateTaskRequest> createValidator)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Create(
        Guid organizationId,
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var validationResult = await createValidator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var task = await taskService.CreateAsync(
            organizationId,
            projectId,
            request,
            userId,
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, task);
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<TaskResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetByProject(
        Guid organizationId,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var tasks = await taskService.GetByProjectAsync(
            organizationId,
            projectId,
            userId,
            cancellationToken);

        if (tasks is null)
        {
            return NotFound();
        }

        return Ok(tasks);
    }

    [HttpPatch("{taskId:guid}/status")]
    public async Task<ActionResult<TaskResponse>> UpdateStatus(
    Guid organizationId,
    Guid projectId,
    Guid taskId,
    [FromBody] UpdateTaskStatusRequest request,
    [FromServices] IValidator<UpdateTaskStatusRequest> validator,
    CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var validation = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var task = await taskService.UpdateStatusAsync(
            organizationId,
            projectId,
            taskId,
            request,
            userId,
            cancellationToken);

        return Ok(task);
    }

    [HttpPatch("{taskId:guid}/priority")]
    public async Task<ActionResult<TaskResponse>> UpdatePriority(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskPriorityRequest request,
        [FromServices] IValidator<UpdateTaskPriorityRequest> validator,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var validation = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var task = await taskService.UpdatePriorityAsync(
            organizationId,
            projectId,
            taskId,
            request,
            userId,
            cancellationToken);

        return Ok(task);
    }

    [HttpPatch("{taskId:guid}/assignee")]
    public async Task<ActionResult<TaskResponse>> Assign(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        [FromBody] AssignTaskRequest request,
        [FromServices] IValidator<AssignTaskRequest> validator,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var validation = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var task = await taskService.AssignAsync(
            organizationId,
            projectId,
            taskId,
            request,
            userId,
            cancellationToken);

        return Ok(task);
    }
}