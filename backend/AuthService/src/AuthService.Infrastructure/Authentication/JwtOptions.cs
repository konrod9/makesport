namespace AuthService.Infrastructure.Authentication;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    
    public string Audience { get; set; } = string.Empty;
    
    public string Secret { get; set; } = string.Empty;

    public int AccessTokenExpirationMinutes { get; set; } = 15;

    public int RefreshTokenExpirationDays { get; set; } = 30;
}