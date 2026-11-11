using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Modsen.Portal.Core.Api.Converters;

public sealed partial class TrimmingConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is not null)
        {
            value = GetInvisibleCharacterRegex().Replace(value, " ");
            value = value.Trim();
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value);
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex GetInvisibleCharacterRegex();
}
