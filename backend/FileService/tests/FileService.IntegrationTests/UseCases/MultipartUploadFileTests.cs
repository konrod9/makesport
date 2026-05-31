using System.Net.Http.Json;
using FileService.Application.HttpCommunication;
using FileService.Application.UseCases.StartMultipartUpload;
using FileService.IntegrationTests.Infrastructure;

namespace FileService.IntegrationTests.UseCases;

public class MultipartUploadFileTests : FileServiceTestsBase
{
    public MultipartUploadFileTests(IntegrationTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task MultipartUpload_FullCycle_PersistsMediaFile()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new StartMultipartUploadRequest(
            "file.mp4",
            "video",
            "video/mp4",
            10000,
            "venue",
            Guid.NewGuid());

        var response = await AppHttpClient.PostAsJsonAsync("multipart-upload", request, cancellationToken);

        var result = await response.HandleResponseAsync<StartMultipartUploadResponse>(cancellationToken: cancellationToken);
    }
}