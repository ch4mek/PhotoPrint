using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;

namespace Modsen.Portal.Certificate.UseCases.CertificatePortalSettings;

internal interface ICertificatePortalSettingsAccessor
{
    Task<CertificatePortalSettingsDto> GetSettings(CancellationToken token);
}

internal class CertificatePortalSettingsAccessor :
    BasePortalSettingsAccessor<CertificatePortalSettingsDto>,
    ICertificatePortalSettingsAccessor
{
    public CertificatePortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
        : base(portalSettingsAccessor) {}

    protected override CertificatePortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new CertificatePortalSettingsDto
        {
            MonthYearFormat = Get(PortalSettingNames.MonthYearFormat).GetString(),
            MonthDayYearFormat = Get(PortalSettingNames.MonthDayYearFormat).GetString()
        };
    }

    protected override string[] GetSettingNames()
    {
        return
        [
            PortalSettingNames.MonthDayYearFormat,
            PortalSettingNames.MonthYearFormat
        ];
    }
}
