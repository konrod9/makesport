using MakeSport.Application;
using MakeSport.Domain.Venues;
using Microsoft.EntityFrameworkCore;

namespace MakeSport.Infrastructure.Postgres.Database;

public class VenueDbContext(DbContextOptions<VenueDbContext> options) : DbContext(options), IVenuesReadDbContext
{
    public DbSet<Venue> Venues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VenueDbContext).Assembly);
    }

    public IQueryable<Venue> VenuesQuery => Venues.AsNoTracking().AsQueryable();
}