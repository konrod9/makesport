namespace AuthService.Application;

public record AuthUserDto(Guid Id, string Email, string FirstName, string LastName, string Role, string UserName);