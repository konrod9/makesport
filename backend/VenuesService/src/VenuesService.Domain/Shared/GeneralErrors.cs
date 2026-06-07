namespace VenuesService.Domain.Shared;

public static class GeneralErrors
{
    public static Error ValueIsInvalid(string? name = null, string? message = null)
    {
        var label = name ?? "значение";
        var errorMessage = message ?? $"{label} недействительно";
        return Error.Validation("value.is.invalid", errorMessage, name);
    }
    
    public static Error ValueIsRequired(string? name = null)
    {
        var label = name ?? string.Empty;
        return Error.Validation("length.is.invalid", $"Поле {label} обязательно");
    }
    
    public static Error NotFound(Guid? id = null, string? name = null)
    {
        var forId = id == null ? string.Empty : $" по Id '{id}'";
        return Error.NotFound("record.not.found", $"{name ?? "запись"} не найдена{forId}");
    }
}