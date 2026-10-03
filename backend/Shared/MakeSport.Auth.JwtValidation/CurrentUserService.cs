using System.Security.Claims;
using MakeSport.Auth.JwtValidation.Abstractions;
using Microsoft.AspNetCore.Http;

namespace MakeSport.Auth.JwtValidation;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var user = accessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            string? raw = user.FindFirstValue("sub")
                          ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? user.FindFirstValue(ClaimTypes.Name);

            return Guid.TryParse(raw, out Guid id) ? id : null;
        }
    }
}