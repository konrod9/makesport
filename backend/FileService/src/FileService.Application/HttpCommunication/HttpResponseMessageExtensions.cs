using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Application.UseCases.StartMultipartUpload;
using FileService.Domain.Shared;

namespace FileService.Application.HttpCommunication;

public static class HttpResponseMessageExtensions
{
    public static async Task<Result<TResponse, Error>> HandleResponseAsync<TResponse>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        try
        {
            var startMultipartUploadResponse = await response.Content
                .ReadFromJsonAsync<Envelope<TResponse>>(cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return startMultipartUploadResponse?.Error ?? GeneralErrors.Failure($"Error while reading response");
            }

            if (startMultipartUploadResponse is null)
            {
                return GeneralErrors.Failure($"Error while reading response");
            }

            if (startMultipartUploadResponse.Error is not null)
            {
                return startMultipartUploadResponse.Error;
            }

            if (startMultipartUploadResponse.Result is null)
            {
                return GeneralErrors.Failure($"Error while reading response");
            }

            return startMultipartUploadResponse.Result;
        }
        catch
        {
            return GeneralErrors.Failure($"Error while reading response");
        }
    }

    public static async Task<UnitResult<Error>> HandleResponseAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var startMultipartUploadResponse = await response.Content
                .ReadFromJsonAsync<Envelope>(cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return startMultipartUploadResponse?.Error ?? GeneralErrors.Failure($"Error while reading response");
            }

            if (startMultipartUploadResponse is null)
            {
                return GeneralErrors.Failure($"Error while reading response");
            }

            if (startMultipartUploadResponse.Error is not null)
            {
                return startMultipartUploadResponse.Error;
            }

            return startMultipartUploadResponse.Result is null 
                ? GeneralErrors.Failure($"Error while reading response") 
                : UnitResult.Success<Error>();
        }
        catch
        {
            return GeneralErrors.Failure($"Error while reading response");
        }
    }
}