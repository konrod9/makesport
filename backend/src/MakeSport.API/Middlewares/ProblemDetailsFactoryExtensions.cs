using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MakeSport.API.Middlewares;

public static class ProblemDetailsFactoryExtensions
{
    public static ProblemDetails CreateFrom(
        this ProblemDetailsFactory factory,
        HttpContext httpContext,
        ValidationException validationException)
    {
        var modelStateDictionary = new ModelStateDictionary();
        
        foreach (var error in validationException.Errors)
        {
            modelStateDictionary.AddModelError(error.PropertyName, error.ErrorCode);
        }
        
        return factory.CreateValidationProblemDetails(
            httpContext: httpContext,
            modelStateDictionary: modelStateDictionary,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Validation failure");
    }
}