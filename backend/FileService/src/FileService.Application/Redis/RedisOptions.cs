namespace FileService.Application.Redis;

public record RedisOptions
{
    public string ServiceUrl { get; init; } = string.Empty;

    public int LocalCacheExpirationMinutes { get; init; } = 5;

    public int ExpirationMinutes { get; init; } = 30;
}