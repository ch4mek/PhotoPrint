namespace Modsen.Portal.Core.Exceptions;

public sealed class ActionNotAllowedException : AppException
{
    public ActionNotAllowedException() { }

    public ActionNotAllowedException(string message)
        : base(message) { }

    public ActionNotAllowedException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
