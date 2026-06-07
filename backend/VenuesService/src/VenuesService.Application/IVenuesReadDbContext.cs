using VenuesService.Domain.Venues;

namespace VenuesService.Application;

public interface IVenuesReadDbContext
{
    IQueryable<Venue> VenuesQuery { get; }
}