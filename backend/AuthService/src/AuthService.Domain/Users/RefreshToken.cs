namespace AuthService.Domain.Users;

public class RefreshToken
{
    private RefreshToken(Guid id, Guid userId, string token, DateTime expiresAt, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        IsRevoked = false;
    }
    
    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public string Token { get; private set; }

    public bool IsRevoked { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    
    public void Revoke() => IsRevoked = true;

    public static RefreshToken Create(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        var id = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;

        return new RefreshToken(id, userId, token, expiresAt, createdAt);
    }
}