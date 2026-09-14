namespace AuthService.Application.UseCases.Register;

public record RegisterResult(AuthUserDto User, string RefreshToken, string AccessToken);