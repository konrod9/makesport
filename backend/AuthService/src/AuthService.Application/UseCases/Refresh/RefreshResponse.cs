namespace AuthService.Application.UseCases.Refresh;

public record RefreshResponse(string AccessToken, string RefreshToken, AuthUserDto User);