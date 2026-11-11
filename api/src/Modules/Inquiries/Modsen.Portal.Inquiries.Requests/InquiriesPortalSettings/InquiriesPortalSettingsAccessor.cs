using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings;

internal interface IInquiriesPortalSettingsAccessor
{
    Task<InquiriesPortalSettingsDto> GetSettings(CancellationToken token);
}

internal class InquiriesPortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor) :
    BasePortalSettingsAccessor<InquiriesPortalSettingsDto>(portalSettingsAccessor),
    IInquiriesPortalSettingsAccessor
{
    protected override string[] GetSettingNames()
    {
        return
        [
            DepartmentSettingNames.SalesDepartmentId,
        ];
    }

    protected override InquiriesPortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new InquiriesPortalSettingsDto
        {
            SalesDepartmentId = Get(DepartmentSettingNames.SalesDepartmentId).GetString(),
        };
    }
}
