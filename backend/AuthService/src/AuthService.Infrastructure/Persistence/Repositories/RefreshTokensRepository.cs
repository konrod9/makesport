using System.Linq.Expressions;
using AuthService.Application;
using AuthService.Application.Interfaces;
using AuthService.Domain.Users;
using AuthService.Infrastructure.Persistence.Database;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AuthService.Infrastructure.Persistence.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly AuthDbContext _dbContext;
    private readonly ILogger<RefreshTokensRepository> _logger;

    public RefreshTokensRepository(AuthDbContext dbContext, ILogger<RefreshTokensRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(refreshToken, cancellationToken);
        return refreshToken.Id;
    }

    public async Task<Result<RefreshToken, Error>> GetByAsync(Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(predicate, cancellationToken);
        if (refreshToken == null)
            return GeneralErrors.NotFound(null, "RefreshToken not found");

        return refreshToken;
    }

    public async Task<Result<int, Error>> SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateException ex) when(ex.InnerException is PostgresException)
        {
            _logger.LogError(ex, "Database update error while saving");
            return AuthServiceErrors.DatabaseError();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while saving");
            return AuthServiceErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while saving");
            return AuthServiceErrors.DatabaseError();
        }
    }
}