namespace Modsen.Portal.Core.Exceptions;

public sealed class InvalidDataAppException : AppException
{
    public InvalidDataAppException() { }

    public InvalidDataAppException(string message)
        : base(message) { }

    public InvalidDataAppException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
