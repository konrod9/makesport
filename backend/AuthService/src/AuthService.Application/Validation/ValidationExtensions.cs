using System.Text.Json;
using FileService.Contracts.Shared;
using FluentValidation.Results;

namespace AuthService.Application.Validation;

public static class ValidationExtensions
{
    public static Error ToError(this ValidationResult validationResult)
    {
        var messages = validationResult.Errors.Select(f =>
            new ErrorMessage(
                f.ErrorCode ?? "value.is.invalid",
                f.ErrorMessage,
                f.PropertyName));
        
        return Error.Validation(messages);
    }
}