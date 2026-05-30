using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Domain.Shared;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public class ChunkSizeCalculator : IChunkSizeCalculator
{
    private readonly S3Options _s3Options;

    public ChunkSizeCalculator(IOptions<S3Options> s3Options)
    {
        _s3Options = s3Options.Value;
    }

    public Result<(long ChunkSize, int TotalChunks), Error> CalculateChunkSize(long fileSize)
    {
        if (_s3Options.RecommendedChunkSizeBytes <= 0 || _s3Options.MaxChunks <= 0)
            return GeneralErrors.ValueIsInvalid("Chunks settings is invalid");

        if (fileSize <= _s3Options.RecommendedChunkSizeBytes)
            return (fileSize, 1);

        var calculatedChunks = (int)Math.Ceiling((double)fileSize / _s3Options.RecommendedChunkSizeBytes);

        var actualChunks = Math.Min(calculatedChunks, _s3Options.MaxChunks);

        var chunkSize = (fileSize + actualChunks) / actualChunks;

        return (chunkSize, actualChunks);
    }
}