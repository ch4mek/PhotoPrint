namespace Modsen.Portal.Core.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException() { }

    public UnauthorizedException(string message)
        : base(message) { }

    public UnauthorizedException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
