using System.Text.Json;
using System.Text.Json.Serialization;

namespace Modsen.Portal.IntegrationTests;

public static class IntegrationTestsConstants
{
    public static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(allowIntegerValues: false)
        },
        PropertyNameCaseInsensitive = true
    };
}
