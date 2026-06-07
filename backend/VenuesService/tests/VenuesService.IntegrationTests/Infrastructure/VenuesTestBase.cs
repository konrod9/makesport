using VenuesService.Infrastructure.Postgres.Database;
using Microsoft.Extensions.DependencyInjection;

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
    
    protected async Task ExecuteInDb(Func<VenuesDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VenuesDbContext>();
        await action(dbContext);
    }
}