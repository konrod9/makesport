namespace MakeSport.Auth.JwtValidation.Abstractions;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}