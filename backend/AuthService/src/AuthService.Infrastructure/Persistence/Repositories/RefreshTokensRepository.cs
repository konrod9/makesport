using System.Linq.Expressions;
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

    public async Task<Result<Guid, Error>> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(refreshToken, cancellationToken);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return refreshToken.Id;
        }
        catch (DbUpdateException ex) when(ex.InnerException is PostgresException)
        {
            _logger.LogError(ex, "Database update error while adding refreshToken with id {RefreshTokenId}", refreshToken.Id);
            return FileServiceErrors.DatabaseError();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while adding refreshToken with id {RefreshTokenId}", refreshToken.Id);
            return FileServiceErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding refreshToken with id {RefreshTokenId}", refreshToken.Id);
            return FileServiceErrors.DatabaseError();
        }
    }

    public async Task<Result<RefreshToken, Error>> GetByAsync(Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(predicate, cancellationToken);
        if (refreshToken == null)
            return GeneralErrors.NotFound(null, "RefreshToken not found");

        return refreshToken;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.SaveChangesAsync(cancellationToken);

    public async Task<Result<Guid, Error>> RevokeAsync(string token, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}