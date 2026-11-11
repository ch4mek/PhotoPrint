using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Modsen.Portal.Notification.UseCases.Core.Converters;

public sealed class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "dd MMM yyyy";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateOnly.ParseExact(value!, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
