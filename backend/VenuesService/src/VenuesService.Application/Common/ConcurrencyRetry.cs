using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VenuesService.Domain.Shared;

namespace VenuesService.Application.Common;

public static class ConcurrencyRetry
{
    public const int MaxAttempts = 3;

    public static async Task<Result<T, Error>> ExecuteAsync<T>(
        Func<CancellationToken, Task<Result<T, Error>>> action,
        IApplicationDbContext dbContext,
        ILogger logger,
        CancellationToken ct)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await action(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entities = string.Join(", ", ex.Entries.Select(e => e.Metadata.ClrType.Name));

                if (attempt >= MaxAttempts)
                {
                    logger.LogWarning("Concurrency conflict after {MaxAttempts} attempts on {Entities}", MaxAttempts,
                        entities);
                    return GeneralErrors.ConcurrencyConflict();
                }

                logger.LogInformation("Concurrency conflict, attempt {Attempt} on {Entities}", attempt, entities);
                dbContext.ClearChangeTracker();
            }
        }
    }
}