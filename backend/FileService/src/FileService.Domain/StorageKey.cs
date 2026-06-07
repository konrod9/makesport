using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace FileService.Domain;

/// <summary>
/// Path to the file in storage, contains key, prefix and location (bucket)
/// </summary>
public sealed record StorageKey
{
    public string Key { get; }

    public string Prefix { get; }

    /// <summary>
    /// In S3 - bucket
    /// </summary>
    public string Location { get; }

    /// <summary>
    /// Contains key with prefix
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Contains key with prefix and location (bucket)
    /// </summary>
    public string FullPath { get; }

    [JsonConstructor]
    private StorageKey(string key, string prefix, string location)
    {
        Key = key;
        Prefix = prefix;
        Location = location;
        Value = string.IsNullOrEmpty(Prefix) ? Key : $"{Prefix}/{Key}";
        FullPath = $"{Location}/{Value}";
    }

    public static Result<StorageKey, Error> Create(string location, string? prefix, string key)
    {
        if (string.IsNullOrWhiteSpace(location))
            return GeneralErrors.ValueIsRequired(nameof(location));

        var normalizedKeyResult = NormalizeSegment(key);
        if (normalizedKeyResult.IsFailure)
            return normalizedKeyResult.Error;

        var normalizedPrefixResult = NormalizePrefix(prefix);
        if (normalizedPrefixResult.IsFailure) 
            return normalizedPrefixResult.Error;
        
        return new StorageKey(normalizedKeyResult.Value, normalizedPrefixResult.Value, location.Trim());
    }

    private static Result<string, Error> NormalizePrefix(string? prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return string.Empty;

        var parts = prefix.Trim().Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        List<string> normalizedParts = [];
        foreach (var part in parts)
        {
            var normalizedPartResult = NormalizeSegment(part);
            if (normalizedPartResult.IsFailure)
                return normalizedPartResult;

            if (!string.IsNullOrEmpty(normalizedPartResult.Value))
                normalizedParts.Add(normalizedPartResult.Value);
        }

        return string.Join('/', normalizedParts);
    }

    private static Result<string, Error> NormalizeSegment(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsInvalid("key");

        var trimmed = value.Trim();

        if (trimmed.Contains('/', StringComparison.Ordinal) || trimmed.Contains('\\', StringComparison.Ordinal))
            return GeneralErrors.ValueIsInvalid("key");

        return trimmed;
    }
}