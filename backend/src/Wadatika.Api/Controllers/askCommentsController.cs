using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Wadatika.Application.DTOs.Tasks;
using Wadatika.Application.Features.Tasks;

namespace Wadatika.Api.Controllers;

[ApiController]
[Authorize]
[Route(
    "api/organizations/{organizationId:guid}/projects/{projectId:guid}/tasks/{taskId:guid}/comments")]
public sealed class TaskCommentsController(
    ITaskCommentService commentService,
    IValidator<CreateTaskCommentRequest> validator)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        [FromBody] CreateTaskCommentRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var comment = await commentService.CreateAsync(
            organizationId,
            projectId,
            taskId,
            request,
            userId,
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, comment);
    }

    [HttpGet]
    public async Task<IActionResult> GetByTask(
        Guid organizationId,
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var comments = await commentService.GetByTaskAsync(
            organizationId,
            projectId,
            taskId,
            userId,
            cancellationToken);

        if (comments is null)
            return NotFound();

        return Ok(comments);
    }
}