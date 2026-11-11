namespace MakeSport.Auth.JwtValidation.Abstractions;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    
    string? DisplayName { get; }

    bool IsInRole(string role);
}