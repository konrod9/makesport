using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Users;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? AvatarId { get; set; }
    
    public UserRole Role { get; set; }
    
    public DateTime CreatedAt { get; set; }
}