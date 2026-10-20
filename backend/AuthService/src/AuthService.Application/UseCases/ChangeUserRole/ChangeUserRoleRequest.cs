using AuthService.Domain.Users;

namespace AuthService.Application.UseCases.ChangeUserRole;

public record ChangeUserRoleRequest(Guid TargetUserId, UserRole NewRole);