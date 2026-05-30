using FileService.API.Configuration;
using FileService.Application;
using FileService.Application.UseCases.CompleteMultipartUpload;
using FileService.Application.UseCases.StartMultipartUpload;
using FileService.Contracts.Requests;
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
    
    [HttpPost("/files/complete-upload")]
    public async Task<EndpointResult> CompleteMultipartUpload(
        [FromBody] CompleteMultipartUploadRequest request,
        [FromServices] CompleteMultipartUploadUseCase useCase,
        CancellationToken ct) => await useCase.Handle(request, ct);
}