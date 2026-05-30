using FileService.Domain;

namespace FileService.Application.Dtos;

public record MediaUrl(StorageKey StorageKey, string PresignedUrl);