namespace Modsen.Portal.Core.Exceptions;

public sealed class ValidationFailedException : Exception
{
    public ValidationFailedException() { }

    public ValidationFailedException(string message) : base(message) { }

    public ValidationFailedException(string message, Exception innerException) : base(message, innerException) { }
}
