namespace Modsen.Portal.TimeTrack.UseCases.Jira.Core;

internal static class JiraUrlValidator
{
    private const string PathDelimiter = "/";

    public static bool IsValid(string url)
    {
        var normalizedUrl = url.Trim();

        if (!normalizedUrl.Contains("://", StringComparison.Ordinal))
        {
            normalizedUrl = $"https://{normalizedUrl}";
        }

        return Uri.TryCreate(normalizedUrl.TrimEnd('/') + PathDelimiter, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
