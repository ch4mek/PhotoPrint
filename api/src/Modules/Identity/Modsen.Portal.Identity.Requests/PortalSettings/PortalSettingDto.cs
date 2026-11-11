using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.Requests.PortalSettings;

internal class PortalSettingDto
{
    public required string Name { get; set; }
    public required string Value { get; set; }
    public AllowedValueType? ValueType { get; set; }
}
