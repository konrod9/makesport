using AuthService.Domain.Users;

namespace AuthService.Application.UseCases.ChangeUserRole;

public record ChangeUserRoleRequestBody(UserRole Role);