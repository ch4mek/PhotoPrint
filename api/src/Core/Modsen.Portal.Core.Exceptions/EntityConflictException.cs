namespace Modsen.Portal.Core.Exceptions;

public sealed class EntityConflictException : AppException
{
    public EntityConflictException() { }

    public EntityConflictException(string message) : base(message) { }

    public EntityConflictException(string message, Exception innerException)
        : base(message, innerException) { }
}
