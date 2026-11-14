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
                if (attempt >= MaxAttempts)
                {
                    logger.LogWarning(ex, "Concurrency conflict after {MaxAttempts} attempts", MaxAttempts);
                    return GeneralErrors.ConcurrencyConflict();
                }
                
                logger.LogInformation(ex, "Concurrency conflict, attempt {Attempt}", attempt);
                dbContext.ClearChangeTracker();
            }
        }
    }
}