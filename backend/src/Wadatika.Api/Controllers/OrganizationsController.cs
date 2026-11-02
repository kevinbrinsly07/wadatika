
using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wadatika.Application.DTOs.Organizations;
using Wadatika.Application.Features.Organizations;

namespace Wadatika.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrganizationsController(
    IOrganizationService organizationService,
    IValidator<CreateOrganizationRequest> createValidator)
    : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(
        typeof(OrganizationResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrganizationResponse>> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var ownerUserId))
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

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var organization = await organizationService.CreateAsync(
            request,
            ownerUserId,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = organization.Id },
            organization);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
    typeof(OrganizationResponse),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationResponse>> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var organization = await organizationService.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        if (organization is null)
        {
            return NotFound();
        }

        return Ok(organization);
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(
    typeof(IReadOnlyList<OrganizationResponse>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<OrganizationResponse>>> GetMine(
    CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var organizations = await organizationService.GetAllForUserAsync(
            userId,
            cancellationToken);

        return Ok(organizations);
    }
}
