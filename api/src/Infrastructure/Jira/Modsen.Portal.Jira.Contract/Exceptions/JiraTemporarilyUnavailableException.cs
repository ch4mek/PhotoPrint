namespace Modsen.Portal.Jira.Contract.Exceptions;

public sealed class JiraTemporarilyUnavailableException : Exception
{
    public JiraTemporarilyUnavailableException() { }

    public JiraTemporarilyUnavailableException(string message)
        : base(message) { }

    public JiraTemporarilyUnavailableException(string message, Exception innerException)
        : base(message, innerException) { }
}
