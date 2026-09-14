using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace AuthService.Application.Interfaces;

public interface IUsersRepository
{
    Task<AppUser?> GetByIdAsync(Guid userId);

    Task<AppUser?> GetByEmailAsync(string email);

    Task<Result<AppUser, Error>> UpdateRoleAsync(AppUser user, UserRole newRole);
    
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken);
}