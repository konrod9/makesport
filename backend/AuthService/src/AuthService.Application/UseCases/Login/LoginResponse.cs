namespace AuthService.Application.UseCases.Login;

public record LoginResponse(string AccessToken, string RefreshToken, AuthUserDto User);