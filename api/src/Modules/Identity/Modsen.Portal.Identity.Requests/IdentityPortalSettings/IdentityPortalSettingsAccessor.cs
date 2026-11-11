using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;

namespace Modsen.Portal.Identity.Requests.IdentityPortalSettings;

internal interface IIdentityPortalSettingsAccessor
{
    Task<IdentityPortalSettingsDto> GetSettings(CancellationToken token);
}

internal class IdentityPortalSettingsAccessor :
    BasePortalSettingsAccessor<IdentityPortalSettingsDto>,
    IIdentityPortalSettingsAccessor
{
    public IdentityPortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
        : base(portalSettingsAccessor) { }
   
    protected override string[] GetSettingNames()
    {
        return
        [
            DatabaseGuardSettingNames.MaxCeoCount,
            DatabaseGuardSettingNames.MaxCtoCount,
            DatabaseGuardSettingNames.MaxHeadOfSpecificDepartmentCount,
            DatabaseGuardSettingNames.MaxGmOfSpecificGroupCount,
            PortalSettingNames.DefaultTimeZone,
            PortalSettingNames.DefaultColorHex,
            PortalSettingNames.EmailDomain,
            PortalSettingNames.DigitalHrContextChangeTimeInMinutes,  
        ];
    }

    protected override IdentityPortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new IdentityPortalSettingsDto
        {
            MaxCeoCount = Get(DatabaseGuardSettingNames.MaxCeoCount).GetInt(),
            MaxCtoCount = Get(DatabaseGuardSettingNames.MaxCtoCount).GetInt(),
            MaxHeadOfSpecificDepartmentCount = Get(DatabaseGuardSettingNames.MaxHeadOfSpecificDepartmentCount).GetInt(),
            MaxGmOfSpecificGroupCount = Get(DatabaseGuardSettingNames.MaxGmOfSpecificGroupCount).GetInt(),
            DigitalHrContextChangeTimeInMinutes = Get(PortalSettingNames.DigitalHrContextChangeTimeInMinutes).GetInt(),
            DefaultTimeZone = Get(PortalSettingNames.DefaultTimeZone).GetString(),
            DefaultColorHex = Get(PortalSettingNames.DefaultColorHex).GetString(),
            EmailDomain = Get(PortalSettingNames.EmailDomain).GetString()
        };
    }
}
