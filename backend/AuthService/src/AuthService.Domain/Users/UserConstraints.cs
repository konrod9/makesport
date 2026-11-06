namespace AuthService.Domain.Users;

public class UserConstraints
{
    public const int PasswordMinLength = 8;
    
    public const int PasswordMaxLength = 100;

    public const int EmailMaxLength = 256;

    public const int NameMaxLength = 50;
    
    public const int UserNameMaxLength = 20;
    
    public const string UserNamePattern = "^[A-Za-z][A-Za-z0-9]*$";
}