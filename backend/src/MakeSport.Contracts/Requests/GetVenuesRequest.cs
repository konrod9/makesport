namespace MakeSport.Contracts.Requests;

public record GetVenuesRequest(string? Search, int Page, int PageSize);