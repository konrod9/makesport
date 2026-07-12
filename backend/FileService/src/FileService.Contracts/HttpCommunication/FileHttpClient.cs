using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Contracts.Dtos;
using FileService.Contracts.Shared;
using Microsoft.Extensions.Logging;

namespace FileService.Contracts.HttpCommunication;

internal sealed class FileHttpClient : IFileCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FileHttpClient> _logger;
    
    public FileHttpClient(HttpClient httpClient, ILogger<FileHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Result<GetMediaAssetsResponse, Error>> GetMediaAssets(GetMediaAssetsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("files/batch", cancellationToken);
            return await response.HandleResponseAsync<GetMediaAssetsResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media assets for {MediaAssetIds}", request.MediaAssetIds);
            return Error.Failure("server.internal", "Failed to request media asset info");
        }
    }

    public async Task<Result<GetByOwnersResponse, Error>> GetByOwners(GetByOwnersRequest request, CancellationToken cancellationToken)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("files/by-owners", request, cancellationToken);

            return await response.HandleResponseAsync<GetByOwnersResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media assets by owners {Owners} and owners type: {OwnerType}", 
                request.OwnerIds, request.OwnerType);
            return Error.Failure("server.internal", "Failed to request media asset info by owners");
        }
    }
}