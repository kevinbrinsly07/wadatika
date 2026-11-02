using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wadatika.Application.DTOs.Projects;
using Wadatika.Application.Features.Projects;

namespace Wadatika.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/projects")]
public sealed class ProjectsController(
    IProjectService projectService,
    IValidator<CreateProjectRequest> createValidator)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(
        typeof(ProjectResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjectResponse>> Create(
        Guid organizationId,
        [FromBody] CreateProjectRequest request,
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
                    group => group
                        .Select(error => error.ErrorMessage)
                        .ToArray());

            return ValidationProblem(
                new ValidationProblemDetails(errors));
        }

        var project = await projectService.CreateAsync(
            organizationId,
            request,
            userId,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByOrganization),
            new { organizationId },
            project);
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ProjectResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>>
        GetByOrganization(
            Guid organizationId,
            CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var projects = await projectService.GetByOrganizationAsync(
            organizationId,
            userId,
            cancellationToken);

        if (projects is null)
        {
            return NotFound();
        }

        return Ok(projects);
    }
}