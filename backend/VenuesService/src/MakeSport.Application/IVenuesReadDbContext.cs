using MakeSport.Domain.Venues;

namespace MakeSport.Application;

public interface IVenuesReadDbContext
{
    IQueryable<Venue> VenuesQuery { get; }
}