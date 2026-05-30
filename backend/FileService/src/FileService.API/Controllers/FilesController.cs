using FileService.API.Configuration;
using FileService.Application;
using FileService.Application.UseCases.StartMultipartUpload;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FileService.API.Controllers;

[ApiController]
[Route("api")]
public class FilesController : ControllerBase
{
    [HttpPost("/files/multipart-upload")]
    public async Task<EndpointResult<StartMultipartUploadResponse>> StartMultipartUpload(
        [FromBody] StartMultipartUploadRequest request,
        [FromServices] StartMultipartUploadUseCase useCase,
        CancellationToken ct) => await useCase.Handle(request, ct);
}