namespace AuthService.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}