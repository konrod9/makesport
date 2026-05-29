using CSharpFunctionalExtensions;
using FileService.Application.Requests;
using FileService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FileService.Application.UseCases.StartMultipartUpload;

public class StartMultipartUploadUseCase
{
    private readonly ILogger<StartMultipartUploadUseCase> _logger;
    private readonly IS3Provider _s3Provider;

    public StartMultipartUploadUseCase(ILogger<StartMultipartUploadUseCase> logger, IS3Provider s3Provider)
    {
        _logger = logger;
        _s3Provider = s3Provider;
    }

    public async Task<Result<Guid, Error>> Handle(StartMultipartUploadRequest request, CancellationToken cancellationToken)
    {
        var startUploadResult = await _s3Provider.StartMultipartUploadAsync();
    }
}