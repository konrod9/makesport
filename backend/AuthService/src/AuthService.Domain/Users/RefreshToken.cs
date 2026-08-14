namespace AuthService.Domain.Users;

public class RefreshToken
{
    public Guid Id { get; private set; }
    
    public Guid UserId { get; set; }
    
    public string Token { get; private set; } = string.Empty;
    
    public bool IsRevoked { get; set; }
    public DateTime ExpiresAt { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}