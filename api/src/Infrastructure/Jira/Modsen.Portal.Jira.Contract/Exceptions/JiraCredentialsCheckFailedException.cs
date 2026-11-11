namespace Modsen.Portal.Jira.Contract.Exceptions;

public sealed class JiraCredentialsCheckFailedException : Exception
{
    public JiraCredentialsCheckFailedException() { }

    public JiraCredentialsCheckFailedException(string message)
        : base(message) { }

    public JiraCredentialsCheckFailedException(string message, Exception innerException)
        : base(message, innerException) { }
}
