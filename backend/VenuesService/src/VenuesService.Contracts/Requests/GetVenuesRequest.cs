using VenuesService.Contracts.Enums;

namespace VenuesService.Contracts.Requests;

public record GetVenuesRequest(
    string? Search,
    string? City,
    string? SportType,
    string? Surface,
    bool? OnlyOpen,
    bool? OnlyFree,
    bool? HasLighting,
    int Page = 1,
    int PageSize = 20,
    VenueSortField SortBy = VenueSortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Desc,
    double? Latitude = null,
    double? Longitude = null);