using MakeSport.Domain.Shared.Enums;

namespace MakeSport.Domain.Shared;

public record Error
{
    public static readonly string SEPARATOR = "||";

    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }
    
    public string Code { get; }
    
    public string Message { get; }
    
    public ErrorType Type { get; }
    
    public string? InvalidField { get; }
    
    public static Error Validation(string code, string message, string? invalidField = null) 
        => new (code, message, ErrorType.Validation, invalidField);
    
    public static Error Failure(string code, string message) =>
        new (code, message, ErrorType.Failure);
    
    public static Error Conflict(string code, string message) =>
        new (code, message, ErrorType.Conflict);
    
    public static Error NotFound(string code, string message) =>
        new (code, message, ErrorType.NotFound);
}