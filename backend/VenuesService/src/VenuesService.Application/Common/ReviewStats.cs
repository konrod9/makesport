using Microsoft.EntityFrameworkCore;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Application.Common;

internal static class ReviewStats
{
    public static async Task<(int Sum, int Count)> GetPublishedAsync(
        IQueryable<Review> reviews,
        VenueId venueId,
        CancellationToken ct)
    {
        var row = await reviews
            .Where(r => r.VenueId == VenueId.Create(venueId)
                        && r.Status == ReviewStatus.Published)
            .GroupBy(r => r.VenueId)
            .Select(g => new
            {
                Sum = g.Sum(x => (int)x.Rating),
                Count = g.Count()
            })
            .FirstOrDefaultAsync(ct);

        return row is null ? (0, 0) : (row.Sum, row.Count);
    }
}