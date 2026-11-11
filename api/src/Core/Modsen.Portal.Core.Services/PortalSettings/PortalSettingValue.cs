using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Services.PortalSettings;

public class PortalSettingValue
{
    public required string Name { get; init; }
    public string? Value { get; init; }
    public AllowedValueType? ValueType { get; init; }

    public int GetInt()
    {
        return !Value.IsNullOrEmpty() && int.TryParse(Value, out var intValue)
            ? intValue
            : throw new ArgumentException($"Portal setting '{Name}' is not configured correctly.");
    }

    public string GetString()
    {
        return !Value.IsNullOrEmpty()
            ? Value
            : throw new ArgumentException($"Portal setting '{Name}' is not configured correctly.");
    }
}
