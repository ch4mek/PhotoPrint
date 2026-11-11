namespace Modsen.Portal.Identity.Requests.IdentityPortalSettings;

internal class IdentityPortalSettingsDto
{
    public int MaxCeoCount { get; init; }
    public int MaxCtoCount { get; init; }
    public int MaxHeadOfSpecificDepartmentCount { get; init; }
    public int MaxGmOfSpecificGroupCount { get; init; }
    public int DigitalHrContextChangeTimeInMinutes { get; init; }
    public string? DefaultTimeZone { get; init; }
    public string? DefaultColorHex { get; init; }
    public string? EmailDomain { get; init; }
}
