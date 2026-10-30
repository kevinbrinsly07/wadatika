using Microsoft.AspNetCore.Mvc;
using Wadatika.Api.Contracts;

namespace Wadatika.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
     [HttpGet]
     [ProducesResponseType(
          typeof(HealthResponse),
          StatusCodes.Status200OK)]
     public IActionResult Get()
     {
          var response = new HealthResponse(
               Status: "Healthy",
               Message: "Wadatika API is running",
               Timestamp: DateTimeOffset.UtcNow
          );

          return Ok(response);
     }
}
