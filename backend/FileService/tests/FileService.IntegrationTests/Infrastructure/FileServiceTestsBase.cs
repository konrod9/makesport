using FileService.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.IntegrationTests.Infrastructure;

public class FileServiceTestsBase : IClassFixture<IntegrationTestsWebFactory>
{
    public const string TestFileName = "test-file.mp4";
    
    protected FileServiceTestsBase(IntegrationTestsWebFactory factory)
    {
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
        Services = factory.Services;
    }

    protected IServiceProvider Services { get; init; }

    protected HttpClient HttpClient { get; init; }

    protected HttpClient AppHttpClient { get; init; }
    
    protected async Task ExecuteInDb(Func<FileServiceDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();
        await action(dbContext);
    }
}