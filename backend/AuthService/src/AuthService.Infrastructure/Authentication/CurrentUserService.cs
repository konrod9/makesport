using System.Security.Claims;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _accessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            string? raw = user.FindFirstValue("sub")
                          ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? user.FindFirstValue(ClaimTypes.Name);

            return Guid.TryParse(raw, out Guid id) ? id : null;
        }
    }
}