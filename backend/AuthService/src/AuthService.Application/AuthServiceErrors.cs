using FileService.Contracts.Shared;

namespace AuthService.Application;

public static class AuthServiceErrors
{
    public static Error InvalidToken() =>
        Error.Failure("invalid.token", "Invalid token");
    
    public static Error InvalidJwtConfiguration() =>
        Error.Failure("auth.jwt.invalid.configuration", "JWT configuration is invalid");
    
    public static Error Unknown() =>
        Error.Failure("unknown.error", "An unknown error occurred.");

    public static Error DatabaseError() =>
        Error.Failure("auth-service.database.error", "Error while accessing the database in auth service");
    
    public static Error OperationCancelled() =>
        Error.Failure("auth-service.cancelled", "Operation was cancelled");
    
    public static Error EmailAlreadyExist() =>
        Error.Conflict("email.already_exist", "Email already exist");
}