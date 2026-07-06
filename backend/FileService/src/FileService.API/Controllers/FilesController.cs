using FileService.API.Configuration;
using FileService.Application.UseCases.CompleteMultipartUpload;
using FileService.Application.UseCases.GetMediaAssetInfo;
using FileService.Application.UseCases.GetMediaAssets;
using FileService.Application.UseCases.StartMultipartUpload;
using FileService.Contracts;
using FileService.Contracts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FileService.API.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    [HttpPost("/multipart-upload")]
    public async Task<EndpointResult<StartMultipartUploadResponse>> StartMultipartUpload(
        [FromBody] StartMultipartUploadRequest request,
        [FromServices] StartMultipartUploadUseCase useCase,
        CancellationToken ct) => await useCase.Handle(request, ct);

    [HttpPost("/complete-upload")]
    public async Task<EndpointResult> CompleteMultipartUpload(
        [FromBody] CompleteMultipartUploadRequest request,
        [FromServices] CompleteMultipartUploadUseCase useCase,
        CancellationToken ct) => await useCase.Handle(request, ct);

    [HttpPost("/batch")]
    public async Task<EndpointResult<GetMediaAssetsResponse>> GetMediaAssets(
        [FromBody] GetMediaAssetsRequest request,
        [FromServices] GetMediaAssetsUseCase useCase,
        CancellationToken ct) => await useCase.Handle(request, ct);
    
    [HttpGet("/{mediaAssetId:guid}")]
    public async Task<EndpointResult<GetMediaAssetInfoDto?>> GetMediaAsset(
        Guid mediaAssetId,
        [FromServices] GetMediaAssetInfoUseCase useCase,
        CancellationToken ct) => await useCase.Handle(mediaAssetId, ct);
}