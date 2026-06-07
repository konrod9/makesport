using VenuesService.Application;
using VenuesService.Domain.Venues;
using Microsoft.EntityFrameworkCore;

namespace VenuesService.Infrastructure.Postgres.Database;

public class VenuesDbContext(DbContextOptions<VenuesDbContext> options) : DbContext(options), IVenuesReadDbContext
{
    public DbSet<Venue> Venues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VenuesDbContext).Assembly);
    }

    public IQueryable<Venue> VenuesQuery => Venues.AsNoTracking().AsQueryable();
}