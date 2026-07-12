namespace FileService.Contracts.Dtos;

public record GetByOwnersRequest(IReadOnlyList<Guid> OwnerIds, string OwnerType);