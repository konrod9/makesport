using FileService.Contracts.Shared;

namespace AuthService.Application;

public static class AuthServiceErrors
{
    public static Error InvalidToken() =>
        Error.Authentication("invalid.token", "Invalid token");
    
    public static Error InvalidJwtConfiguration() =>
        Error.Authentication("auth.jwt.invalid.configuration", "JWT configuration is invalid");
    
    public static Error Unknown() =>
        Error.Failure("unknown.error", "An unknown error occurred.");

    public static Error DatabaseError() =>
        Error.Failure("auth-service.database.error", "Error while accessing the database in auth service");
    
    public static Error OperationCancelled() =>
        Error.Failure("auth-service.cancelled", "Operation was cancelled");
    
    public static Error EmailAlreadyExist() =>
        Error.Conflict("email.already_exist", "Email already exist");

    public static Error UserLockedOut(DateTimeOffset? endDate) =>
        Error.Authorization("user.locked.out", 
            endDate != null 
            ? $"Ошибка доступа. Попробуйте снова после {endDate}" 
            : "Ошибка доступа. Попробуйте позже");

    public static Error UserNotFound() =>
        Error.Authentication("auth.user.not-found", "Пользователь не найден");

    public static Error TargetUserNotFound() =>
        Error.NotFound("auth.user.not-found", "Целевой пользователь не найден");
    
    public static Error CannotDemoteSelf() =>
        Error.Authorization("auth.role.cannot-demote-self", "Нельзя понизить собственную роль администратора");

    public static Error Unauthorized() =>
        Error.Authentication("auth.unauthorized", "Пользователь не авторизован");
}