using System.Text.Json;
using FluentValidation.Results;
using MakeSport.Domain.Shared;

namespace MakeSport.Application.Validation;

public static class ValidationExtensions
{
    public static Error ToError(this ValidationResult validationResult)
    {
        var validationErrors = validationResult.Errors;

        var errors =
            from validationError in validationErrors
            let errorMessage = validationError.ErrorMessage
            let error = JsonSerializer.Deserialize<Error>(errorMessage)
            select error.Messages;
        
        return Error.Validation(errors.SelectMany(e => e));
    }
}