namespace MakeSport.Contracts.Requests;

public record GetVenuesRequest(string? Search, int Page = 1, int PageSize = 100);