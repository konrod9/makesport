namespace AuthService.Application.UseCases.Register;

public record RegisterResponse(AuthUserDto User, string RefreshToken, string AccessToken);