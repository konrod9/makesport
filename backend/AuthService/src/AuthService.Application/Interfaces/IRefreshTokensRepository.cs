using System.Linq.Expressions;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace AuthService.Application.Interfaces;

public interface IRefreshTokensRepository
{
    Task<Result<Guid, Error>> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task<Result<RefreshToken, Error>> GetByAsync(Expression<Func<RefreshToken, bool>> expression,
        CancellationToken cancellationToken = default);
    
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    
    Task<Result<Guid, Error>> RevokeAsync(string token, CancellationToken cancellationToken);
}