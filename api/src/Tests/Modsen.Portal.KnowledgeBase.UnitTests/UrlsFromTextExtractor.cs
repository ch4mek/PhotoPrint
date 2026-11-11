using System.Reflection;
using Modsen.Portal.KnowledgeBase.Requests.Core;
using Xunit.Sdk;

namespace Modsen.Portal.KnowledgeBase.UnitTests;

public class UrlsFromTextExtractorTests
{
    private sealed class InterfacesInline : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return [[new UrlsFromTextExtractor()]];
        }
    }

    [Theory]
    [InterfacesInline]
    internal void ExtractUrls(IUrlsFromTextExtractor extractor)
    {
        extractor.ExtractUrls("").Should().BeEmpty();
        extractor.ExtractUrls("https://goo.gle").Should().BeEquivalentTo(["https://goo.gle"]);
        extractor.ExtractUrls(""" "url": "https://goo.gle" and "url":"http://ya.ru" """).Should()
            .BeEquivalentTo(["https://goo.gle", "http://ya.ru"]);
    }
}
