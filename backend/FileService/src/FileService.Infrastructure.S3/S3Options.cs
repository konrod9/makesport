namespace FileService.Infrastructure.S3;

public record S3Options
{
    public string ServiceUrl { get; init; } = string.Empty;
    
    public string AccessKey { get; init; } = string.Empty;
    
    public string SecretKey { get; init; } = string.Empty;
    
    public bool WithSsl { get; init; }
    
    public int DownloadUrlExpirationDays { get; init; } = 6;
    
    public IReadOnlyList<string> RequiredBuckets { get; init; } = Array.Empty<string>();
    
    public double UploadUrlExpirationHours { get; init; } = 1;
    
    public int MaxConcurrentRequests { get; init; } = 50;
    
    public long RecommendedChunkSizeBytes { get; init; } = 100 * 1024 * 1024; // 100 MB
    
    public int MaxChunks { get; init; } = 100;
}