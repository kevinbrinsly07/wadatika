using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wadatika.Application.DTOs.Teams;
using Wadatika.Application.Features.Teams;

namespace Wadatika.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/organizations/{organizationId:guid}/teams")]
public sealed class TeamsController(
    ITeamService teamService,
    IValidator<CreateTeamRequest> createValidator)
    : ControllerBase
{
     [HttpPost]
     [ProducesResponseType(
         typeof(TeamResponse),
         StatusCodes.Status201Created)]
     [ProducesResponseType(StatusCodes.Status400BadRequest)]
     [ProducesResponseType(StatusCodes.Status401Unauthorized)]
     [ProducesResponseType(StatusCodes.Status404NotFound)]
     public async Task<ActionResult<TeamResponse>> Create(
         Guid organizationId,
         [FromBody] CreateTeamRequest request,
         CancellationToken cancellationToken)
     {
          var userIdValue = User.FindFirstValue("sub");

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

          var team = await teamService.CreateAsync(
              organizationId,
              request,
              userId,
              cancellationToken);

          return StatusCode(
              StatusCodes.Status201Created,
              team);
     }

     [HttpGet]
     [ProducesResponseType(typeof(IReadOnlyList<TeamResponse>), StatusCodes.Status200OK)]
     [ProducesResponseType(StatusCodes.Status401Unauthorized)]
     [ProducesResponseType(StatusCodes.Status404NotFound)]
     public async Task<ActionResult<IReadOnlyList<TeamResponse>>> GetByOrganization(
     Guid organizationId,
     CancellationToken cancellationToken)
     {
          var userIdValue = User.FindFirstValue("sub");

          if (!Guid.TryParse(userIdValue, out var userId))
          {
               return Unauthorized();
          }

          var teams = await teamService.GetByOrganizationAsync(
              organizationId,
              userId,
              cancellationToken);

          return Ok(teams);
     }
}