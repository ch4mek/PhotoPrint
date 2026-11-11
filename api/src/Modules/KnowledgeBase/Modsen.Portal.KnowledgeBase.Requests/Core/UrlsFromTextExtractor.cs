using System.Text.RegularExpressions;

namespace Modsen.Portal.KnowledgeBase.Requests.Core;

internal interface IUrlsFromTextExtractor
{
    string[] ExtractUrls(string text);
}

internal class UrlsFromTextExtractor : IUrlsFromTextExtractor
{
    public string[] ExtractUrls(string text)
    {
        var regex = new Regex(@"https?://[^\s""']+", RegexOptions.None, TimeSpan.FromSeconds(1));

        var matches = regex.Matches(text);

        var urls = matches.Select(m => m.Groups[0].Value).ToArray();

        return urls;
    }
}
