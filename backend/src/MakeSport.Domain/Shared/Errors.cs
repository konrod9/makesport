namespace MakeSport.Domain.Shared;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null) =>
            Error.Validation("value.is.invalid", $"{name ?? "value"} is invalid.");

        public static Error NotFound(Guid? id = null, string? name = null)
        {
            var forId = id == null ? "" : $" for Id '{id}'";
            return Error.NotFound("record.not.found", $"{name ?? "record"} not found{forId}");
        }
        
        public static Error ValueIsRequired(string? name = null) =>
            Error.Validation("value.is.required", $"{name ?? "value"} is required.");
    }
}