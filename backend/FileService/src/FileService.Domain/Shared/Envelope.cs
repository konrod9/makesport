using System.Text.Json.Serialization;

namespace FileService.Domain.Shared;

public record Envelope
{
    public object? Result { get; }
    
    public Error? Error { get; }
    
    public bool IsError => Error != null;
    
    public DateTime TimeGenerated { get; }
    
    [JsonConstructor]
    private Envelope(object? result, Error? error)
    {
        Result = result;
        Error = error;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope Ok(object? result = null) => new(result, null);

    public static Envelope Fail(Error? error = null) => new (null, error);
}