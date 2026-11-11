using System.Text.Json;
using System.Text.Json.Serialization;

namespace Modsen.Portal.Core.Api.Converters;

public sealed partial class SideTrimmingConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (value is not null)
        {
            value = value.Trim();
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value);
    }
}
