namespace FileService.Domain.Shared.Exceptions;

public class AuthenticationException : Exception
{
    public Error Error { get; } = null!;

    public AuthenticationException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public AuthenticationException()
    {
    }

    public AuthenticationException(string message)
        : base(message)
    {
    }

    public AuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}