using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MakeSport.API.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, ProblemDetailsFactory problemDetailsFactory)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            var httpStatusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            
            // TODO: Add logging error

            ProblemDetails problemDetails;
            switch (exception)
            {
                case ValidationException validationException:
                    problemDetails = problemDetailsFactory.CreateFrom(httpContext, validationException);
                    // TODO: Add logging information
                    break;
                
                default:
                    problemDetails = problemDetailsFactory.CreateProblemDetails(
                        httpContext,
                        StatusCodes.Status500InternalServerError, "Unhandled error! Please contact us.");
                    // TODO: Add logging error
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, problemDetailsFactory.GetType());
        }
    }
}