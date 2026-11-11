namespace Modsen.Portal.Core.Authentication.External;

public sealed class ExternalUnauthorizedException : Exception
{
    public ExternalUnauthorizedException() { }

    public ExternalUnauthorizedException(string message) : base(message) { }

    public ExternalUnauthorizedException(string message, Exception innerException)
        : base(message, innerException) { }
}
