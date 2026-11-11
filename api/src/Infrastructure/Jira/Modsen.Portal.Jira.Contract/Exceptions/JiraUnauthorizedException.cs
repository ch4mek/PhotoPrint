namespace Modsen.Portal.Jira.Contract.Exceptions;

public sealed class JiraUnauthorizedException : Exception
{
    public JiraUnauthorizedException() { }

    public JiraUnauthorizedException(string message)
        : base(message) { }

    public JiraUnauthorizedException(string message, Exception innerException)
        : base(message, innerException) { }
}
