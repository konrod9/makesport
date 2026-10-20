using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace AuthService.Application.Interfaces;

public interface IIdentityService
{
    Task<AppUser?> GetByIdAsync(Guid userId);

    Task<AppUser?> GetByEmailAsync(string email);

    Task<Result<AppUser, Error>> UpdateRoleAsync(AppUser user, UserRole newRole);
    
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken);

    Task<Result<AppUser, Error>> CreateAsync(AppUser user, string password);

    Task<bool> IsLockedOutAsync(AppUser user);

    Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user);

    Task<bool> CheckPasswordAsync(AppUser user, string password);
}