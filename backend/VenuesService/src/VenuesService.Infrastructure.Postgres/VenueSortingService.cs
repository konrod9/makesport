using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using VenuesService.Application.UseCases.GetVenues;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Venues;

namespace VenuesService.Infrastructure.Postgres;

public class VenueSortingService : IVenueSortingService
{
    public IQueryable<Venue> ApplySort(
        IQueryable<Venue> query,
        VenueSortField sortBy,
        SortDirection sortDirection,
        double? latitude,
        double? longitude)
    {
        if (sortBy == VenueSortField.Distance && latitude.HasValue && longitude.HasValue)
        {
            var origin = new Point(longitude.Value, latitude.Value) { SRID = 4326 };

            Expression<Func<Venue, double>> distance = v =>
                EF.Functions.Distance(
                    v.Location,
                    origin,
                    false);
            
            return sortDirection == SortDirection.Asc
                ? query.OrderBy(distance).ThenBy(v => v.Id)
                : query.OrderByDescending(distance).ThenBy(v => v.Id);
        }
        
        return sortBy switch
        {
            VenueSortField.Title when sortDirection == SortDirection.Asc =>
                query.OrderBy(v => v.Title).ThenBy(v => v.Id),

            VenueSortField.Title =>
                query.OrderByDescending(v => v.Title).ThenBy(v => v.Id),

            VenueSortField.CreatedAt when sortDirection == SortDirection.Asc =>
                query.OrderBy(v => v.CreatedAt).ThenBy(v => v.Id),

            _ => query.OrderByDescending(v => v.CreatedAt).ThenBy(v => v.Id)
        };
    }
}