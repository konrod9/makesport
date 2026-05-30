using CSharpFunctionalExtensions;
using FileService.Domain.Shared;

namespace FileService.Application.FilesStorage;

public interface IChunkSizeCalculator
{
    Result<(long ChunkSize, int TotalChunks), Error> CalculateChunkSize(long fileSize);
}