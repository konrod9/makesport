using AuthService.Application.Interfaces;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly UserManager<AppUser> _userManager;

    public UsersRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AppUser?> GetByIdAsync(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email); 
    }

    public async Task<Result<AppUser, Error>> UpdateRoleAsync(AppUser user, UserRole newRole)
    {
        user.Role = newRole;
        
        IdentityResult result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) return user;
        
        var errorMessages = result.Errors
            .Select(e => new ErrorMessage(e.Code, e.Description))
            .ToArray();

        return Error.Failure(errorMessages);

    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Result<AppUser, Error>> CreateAsync(AppUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded) 
            return user;

        ErrorMessage[] errorMessages = result.Errors
            .Select(e => new ErrorMessage(e.Code, e.Description))
            .ToArray();

        return result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail") 
            ? Error.Conflict(errorMessages) 
            : Error.Failure(errorMessages);
    }
}