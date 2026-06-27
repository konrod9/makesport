namespace VenuesService.Contracts.Requests;

public record GetVenuesRequest(string? Search, bool? HasLighting, int Page = 1, int PageSize = 100);