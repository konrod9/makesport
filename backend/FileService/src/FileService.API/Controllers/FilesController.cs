using CSharpFunctionalExtensions;
using FileService.API.Configuration;
using FileService.Application;
using FileService.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FileService.API.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult> UploadFile(
        IFormFile file,
        [FromServices] IS3Provider s3Provider,
        CancellationToken ct)
    {
        var key = $"raw/{Guid.NewGuid()}";
        
        //await s3Provider.UploadFileAsync(file.OpenReadStream(), "pictures", key, file.ContentType, ct);

        return new EndpointResult(UnitResult.Success<Error>());
    }
    
    [HttpGet("/url")]
    public async Task<EndpointResult<string>> GetDownloadUrl(
        [FromQuery] string bucketName,
        [FromQuery] string key,
        [FromServices] IS3Provider s3Provider,
        CancellationToken ct)
    {
        var url = await s3Provider.GenerateDownloadUrlAsync(bucketName, key);
        
        return new EndpointResult<string>(url);
    }
}