using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Contracts.Shared;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public class ChunkSizeCalculator : IChunkSizeCalculator
{
    private readonly FileStorageOptions _fileStorageOptions;

    public ChunkSizeCalculator(IOptions<FileStorageOptions> s3Options)
    {
        _fileStorageOptions = s3Options.Value;
    }

    public Result<(int ChunkSize, int TotalChunks), Error> CalculateChunkSize(long fileSize)
    {
        if (_fileStorageOptions.RecommendedChunkSizeBytes <= 0 || _fileStorageOptions.MaxChunks <= 0)
            return GeneralErrors.ValueIsInvalid("Chunks settings is invalid");

        if (fileSize <= _fileStorageOptions.RecommendedChunkSizeBytes)
            return ((int)fileSize, 1);

        var calculatedChunks = (int)Math.Ceiling((double)fileSize / _fileStorageOptions.RecommendedChunkSizeBytes);

        var actualChunks = Math.Min(calculatedChunks, _fileStorageOptions.MaxChunks);

        var chunkSize = (fileSize + actualChunks) / actualChunks;

        return ((int)chunkSize, actualChunks);
    }
}