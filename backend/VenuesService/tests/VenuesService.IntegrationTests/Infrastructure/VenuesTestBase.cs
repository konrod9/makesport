using VenuesService.Infrastructure.Postgres.Database;
using Microsoft.Extensions.DependencyInjection;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Venues;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.IntegrationTests.Infrastructure;

public class VenuesTestsBase : IClassFixture<IntegrationTestsWebFactory>
{
    protected VenuesTestsBase(IntegrationTestsWebFactory factory)
    {
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
        Services = factory.Services;
    }

    protected IServiceProvider Services { get; init; }

    protected HttpClient HttpClient { get; init; }

    protected HttpClient AppHttpClient { get; init; }
    
    protected async Task<Guid> CreateVenueAsync()
    {
        var venueId = Guid.NewGuid();

        var venue = Venue.Create(
            id: VenueId.Create(venueId),
            title: $"Test venue {venueId}",
            address: Address.Create("Одинцов", "Можайское шоссе", "12А").Value,
            location: Coordinates.Create(55.670916, 37.267876).Value,
            isOpen: true,
            isFree: true,
            hasLighting: true,
            workingHours: WorkingHours.Create("08:00", "22:00").Value,
            surface: "Резиновое",
            sportType: "Баскетбол",
            userId: null,
            description: null);

        await ExecuteInDb(async dbContext =>
        {
            dbContext.Venues.Add(venue);
            await dbContext.SaveChangesAsync();
        });

        return venueId;
    }

    protected async Task<Review> CreateReviewAsync(
        VenueId venueId,
        int rating,
        string? text = "Отличная площадка",
        Guid? authorId = null,
        ReviewStatus? status = ReviewStatus.Published)
    {
        var review = Review.Create(
            id: ReviewId.NewId(),
            venueId: venueId,
            authorId: authorId ?? Guid.NewGuid(),
            authorName: "Test author",
            rating: rating,
            text: text);

        if (status == ReviewStatus.Hidden)
        {
            review.Value.Hide();
        }

        await ExecuteInDb(async db =>
        {
            db.Reviews.Add(review.Value);
            await db.SaveChangesAsync();
        });
        
        return review.Value;
    }

    protected async Task ExecuteInDb(Func<VenuesDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VenuesDbContext>();
        await action(dbContext);
    }
}