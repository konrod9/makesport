using VenuesService.Contracts.Enums;
using VenuesService.Domain.Venues;

namespace VenuesService.Application.UseCases.GetVenues;

public interface IVenueSortingService
{
    IQueryable<Venue> ApplySort(
        IQueryable<Venue> query,
        VenueSortField sortBy,
        SortDirection sortDirection,
        double? latitude,
        double? longitude);
}