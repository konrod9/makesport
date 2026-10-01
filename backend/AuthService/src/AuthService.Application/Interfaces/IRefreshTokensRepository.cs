using System.Linq.Expressions;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace AuthService.Application.Interfaces;

public interface IRefreshTokensRepository
{
    Task<Guid> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task<Result<RefreshToken, Error>> GetByAsync(Expression<Func<RefreshToken, bool>> expression,
        CancellationToken cancellationToken = default);
    
    Task<Result<int, Error>> SaveAsync(CancellationToken cancellationToken = default);
}