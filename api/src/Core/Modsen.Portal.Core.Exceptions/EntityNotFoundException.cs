namespace Modsen.Portal.Core.Exceptions;

public sealed class EntityNotFoundException : AppException
{
    public EntityNotFoundException() { }

    public EntityNotFoundException(string message)
        : base(message) { }

    public EntityNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
