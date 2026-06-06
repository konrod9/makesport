using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Application.HttpCommunication;
using FileService.Application.UseCases.CompleteMultipartUpload;
using FileService.Application.UseCases.StartMultipartUpload;
using FileService.Contracts;
using FileService.Domain;
using FileService.Domain.Shared;
using FileService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FileService.IntegrationTests.UseCases;

public class MultipartUploadFileTests : FileServiceTestsBase
{
    public MultipartUploadFileTests(IntegrationTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task MultipartUpload_FullCycle_PersistsMediaFile()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new FileInfo(Path.Combine(AppContext.BaseDirectory, "Resources", TestFileName));

        // Act
        var startMultipartUploadResponse = await StartMultipartUpload(fileInfo, cancellationToken);

        var partEtags = await UploadChunks(fileInfo, startMultipartUploadResponse, cancellationToken);

        var completeMultipartResult = await CompleteMultipartUpload(startMultipartUploadResponse, partEtags, cancellationToken);
        
        // Assert
        Assert.True(completeMultipartResult.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var mediaAsset = await dbContext.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartUploadResponse.MediaAssetId,
                    cancellationToken: cancellationToken);

            Assert.NotNull(mediaAsset);
            Assert.Equal(MediaStatus.Uploaded, mediaAsset.Status);
        });
    }

    private async Task<StartMultipartUploadResponse> StartMultipartUpload(FileInfo fileInfo,
        CancellationToken cancellationToken)
    {
        var request = new StartMultipartUploadRequest(
            fileInfo.Name,
            "video",
            "video/mkv",
            fileInfo.Length,
            "venue",
            Guid.NewGuid());

        var startMultipartResponse =
            await AppHttpClient.PostAsJsonAsync("multipart-upload", request, cancellationToken);

        var startMultipartResult = await startMultipartResponse
            .HandleResponseAsync<StartMultipartUploadResponse>(cancellationToken: cancellationToken);

        Assert.True(startMultipartResult.IsSuccess);
        Assert.NotNull(startMultipartResult.Value.UploadId);

        await ExecuteInDb(async dbContext =>
        {
            var mediaAsset = await dbContext.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == startMultipartResult.Value.MediaAssetId,
                    cancellationToken: cancellationToken);

            Assert.NotNull(mediaAsset);
            Assert.Equal(MediaStatus.Uploading, mediaAsset.Status);
        });

        return startMultipartResult.Value;
    }

    private async Task<IReadOnlyList<PartETagDto>> UploadChunks(
        FileInfo fileInfo,
        StartMultipartUploadResponse startMultipartUploadResponse,
        CancellationToken cancellationToken)
    {
        await using var stream = fileInfo.OpenRead();

        var parts = new List<PartETagDto>();

        foreach (var url in startMultipartUploadResponse.ChunkUploadUrls.OrderBy(c => c.PartNumber))
        {
            byte[] chunk = new byte[startMultipartUploadResponse.ChunkSize];
            int bytesRead = await stream.ReadAsync(chunk.AsMemory(0, startMultipartUploadResponse.ChunkSize), cancellationToken);
            if (bytesRead == 0)
                break;

            var content = new ByteArrayContent(chunk.Take(bytesRead).ToArray());

            // Ответ от S3 хранилища
            var response = await HttpClient.PutAsync(url.UploadUrl, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Put chunk failed with {response.StatusCode}. Content: {errorContent}");
            }

            var etag = response.Headers.ETag?.Tag.Trim('"');
            parts.Add(new PartETagDto(url.PartNumber, etag!));
        }

        return parts;
    }

    private async Task<UnitResult<Error>> CompleteMultipartUpload(
        StartMultipartUploadResponse startMultipartUploadResponse,
        IEnumerable<PartETagDto> partEtags,
        CancellationToken cancellationToken)
    {
        var completeRequest = new CompleteMultipartUploadRequest(
            startMultipartUploadResponse.MediaAssetId,
            startMultipartUploadResponse.UploadId,
            partEtags.ToList());

        var completeResponse =
            await AppHttpClient.PostAsJsonAsync("complete-upload", completeRequest, cancellationToken);

        var completeMultipartResult = await completeResponse
            .HandleResponseAsync(cancellationToken);

        return completeMultipartResult;
    }
}