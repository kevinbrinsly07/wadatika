using FluentValidation;
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
     [HttpPost]
     [ProducesResponseType(
         typeof(OrganizationResponse),
         StatusCodes.Status201Created)]
     [ProducesResponseType(StatusCodes.Status400BadRequest)]
     [ProducesResponseType(StatusCodes.Status409Conflict)]
     public async Task<ActionResult<OrganizationResponse>> Create(
         [FromBody] CreateOrganizationRequest request,
         CancellationToken cancellationToken)
     {
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

          try
          {
               var organization = await organizationService.CreateAsync(
                   request,
                   cancellationToken);

               return CreatedAtAction(
                   nameof(GetById),
                   new { id = organization.Id },
                   organization);
          }
          catch (OrganizationSlugAlreadyExistsException exception)
          {
               return Conflict(new ProblemDetails
               {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Organization already exists",
                    Detail = exception.Message
               });
          }
     }

     [HttpGet("{id:guid}")]
     [ProducesResponseType(
         typeof(OrganizationResponse),
         StatusCodes.Status200OK)]
     [ProducesResponseType(StatusCodes.Status404NotFound)]
     public async Task<ActionResult<OrganizationResponse>> GetById(
         Guid id,
         CancellationToken cancellationToken)
     {
          var organization = await organizationService.GetByIdAsync(
              id,
              cancellationToken);

          if (organization is null)
          {
               return NotFound();
          }

          return Ok(organization);
     }
}