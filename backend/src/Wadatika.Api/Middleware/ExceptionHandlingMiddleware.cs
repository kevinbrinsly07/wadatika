using Microsoft.AspNetCore.Mvc;
using Wadatika.Application.Features.Authentication;
using Wadatika.Application.Features.Organizations;

namespace Wadatika.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
     public async Task InvokeAsync(HttpContext context)
     {
          try
          {
               await next(context);
          }
          catch (Exception exception)
          {
               logger.LogError(
                   exception,
                   "An unhandled exception occurred while processing {Path}",
                   context.Request.Path);

               await HandleExceptionAsync(context, exception);
          }
     }

     private static async Task HandleExceptionAsync(
         HttpContext context,
         Exception exception)
     {
          var problemDetails = exception switch
          {
               OrganizationSlugAlreadyExistsException => new ProblemDetails
               {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Organization already exists",
                    Detail = exception.Message
               },

               EmailAlreadyRegisteredException => new ProblemDetails
               {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Email already registered",
                    Detail = exception.Message
               },

               InvalidCredentialsException => new ProblemDetails
               {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Invalid credentials",
                    Detail = exception.Message
               },

               _ => new ProblemDetails
               {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred",
                    Detail = "An unexpected error occurred while processing your request."
               }
          };

          context.Response.StatusCode =
              problemDetails.Status ?? StatusCodes.Status500InternalServerError;

          context.Response.ContentType = "application/problem+json";

          await context.Response.WriteAsJsonAsync(problemDetails);
     }
}