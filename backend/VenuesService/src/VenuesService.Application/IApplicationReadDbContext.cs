using VenuesService.Domain.Reviews;
using VenuesService.Domain.Venues;

namespace VenuesService.Application;

public interface IApplicationReadDbContext
{
    IQueryable<Venue> VenuesQuery { get; }
    IQueryable<Review> ReviewsQuery { get; }
}