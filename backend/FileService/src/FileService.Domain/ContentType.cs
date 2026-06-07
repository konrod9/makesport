using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace FileService.Domain;

public sealed record ContentType
{
    public string Value { get; }

    public MediaType Category { get; }

    private ContentType(string value, MediaType category)
    {
        Value = value;
        Category = category;
    }

    public static Result<ContentType, Error> Create(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return GeneralErrors.ValueIsRequired(nameof(contentType));

        var mediaType = contentType.ToLowerInvariant() switch
        {
            var ct when ct.Contains("video", StringComparison.InvariantCultureIgnoreCase) => MediaType.Video,
            var ct when ct.Contains("image", StringComparison.InvariantCultureIgnoreCase) => MediaType.Image,
            var ct when ct.Contains("audio", StringComparison.InvariantCultureIgnoreCase) => MediaType.Audio,
            var ct when ct.Contains("document", StringComparison.InvariantCultureIgnoreCase) => MediaType.Document,
            _ => MediaType.Unknown
        };

        return new ContentType(contentType, mediaType);
    }
}