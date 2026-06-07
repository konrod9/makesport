using VenuesService.Domain.Shared.Enums;

namespace VenuesService.Domain.Shared;

public record ErrorMessage(string Code, string Message, string? InvalidField = null);

public record Error
{
    public IReadOnlyList<ErrorMessage> Messages { get; } = [];

    public ErrorType Type { get; }

    private Error(IReadOnlyList<ErrorMessage> messages, ErrorType type)
    {
        Messages = messages.ToArray();
        Type = type;
    }
    
    private Error(IEnumerable<ErrorMessage> messages, ErrorType type)
    {
        Messages = messages.ToArray();
        Type = type;
    }
    
    public string GetMessage() => string.Join(";", Messages.Select(m => m.ToString()));
    
    public static Error Validation(string code, string message, string? invalidField = null) =>
        new([new ErrorMessage(code, message, invalidField)], ErrorType.Validation);
    
    public static Error NotFound(string code, string message, string? invalidField = null) =>
        new([new ErrorMessage(code, message, invalidField)], ErrorType.NotFound);
    
    public static Error Failure(string code, string message, string? invalidField = null) =>
        new([new ErrorMessage(code, message, invalidField)], ErrorType.Failure);
    
    public static Error Conflict(string code, string message, string? invalidField = null) =>
        new([new ErrorMessage(code, message, invalidField)], ErrorType.Conflict);
    
    public static Error Authentication(string code, string message, string? invalidField = null) =>
        new([new ErrorMessage(code, message, invalidField)], ErrorType.Authentication);
    
    public static Error Authorization(string code, string message, string? invalidField = null) =>
        new([new ErrorMessage(code, message, invalidField)], ErrorType.Authorization);
    
    public static Error Validation(params IEnumerable<ErrorMessage> messages) =>
        new(messages, ErrorType.Validation);
    
    public static Error NotFound(params IEnumerable<ErrorMessage> messages) =>
        new(messages, ErrorType.NotFound);

    public static Error Failure(params IEnumerable<ErrorMessage> messages) =>
        new(messages, ErrorType.Failure);

    public static Error Conflict(params IEnumerable<ErrorMessage> messages) =>
        new(messages, ErrorType.Conflict);

    public static Error Authentication(params IEnumerable<ErrorMessage> messages) =>
        new(messages, ErrorType.Authentication);

    public static Error Authorization(params IEnumerable<ErrorMessage> messages) =>
        new(messages, ErrorType.Authorization);
}