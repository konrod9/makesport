using CSharpFunctionalExtensions;
using FileService.Domain.Shared;

namespace FileService.Domain;

public sealed record FileName
{
    public string Value { get; }
    
    public string Name { get; }

    public string Extension { get; }

    private FileName(string name, string extension)
    {
        Name = name;
        Extension = extension;
        Value = $"{name}.{extension}";
    }

    public static Result<FileName, Error> Create(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return GeneralErrors.ValueIsRequired(nameof(fileName));

        var lastDot = fileName.LastIndexOf('.');
        if (lastDot == -1 || lastDot == fileName.Length - 1)
            return GeneralErrors.ValueIsInvalid(nameof(fileName), "File must have an extension");

        var extension = fileName[(lastDot + 1)..].ToLowerInvariant();
        return new FileName(fileName, extension);
    }
}