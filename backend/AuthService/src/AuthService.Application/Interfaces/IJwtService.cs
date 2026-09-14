using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace AuthService.Application.Interfaces;

public interface IJwtService
{
    Result<string, Error> GenerateAccessToken(AppUser user);
    
    Result<RefreshToken, Error> GenerateRefreshToken(Guid userId);
    
    Result<bool, Error> ValidateAccessToken(string accessToken);
}