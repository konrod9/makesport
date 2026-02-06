using MakeSport.Domain.Venues;
using Microsoft.EntityFrameworkCore;

namespace MakeSport.Infrastructure.Postgres;

public class VenueDbContext(DbContextOptions<VenueDbContext> options) : DbContext(options)
{
    public DbSet<Venue> Venue { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VenueDbContext).Assembly);
    }
}