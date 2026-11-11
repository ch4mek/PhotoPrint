using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

public interface ITeamBoardPortalSettingsAccessor
{
    Task<TeamBoardPortalSettingsDto> GetSettings(CancellationToken token);
}

public class TeamBoardPortalSettingsAccessor :
    BasePortalSettingsAccessor<TeamBoardPortalSettingsDto>,
    ITeamBoardPortalSettingsAccessor
{
    public TeamBoardPortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
        : base(portalSettingsAccessor) { }

    protected override string[] GetSettingNames()
    {
        return
        [
            DepartmentSettingNames.SystemAdministrationDepartmentId,
            DepartmentSettingNames.SalesDepartmentId,
            PerformanceReviewSettingNames.OverdueDaysCount,
            PerformanceReviewSettingNames.UpcomingIminentBorderDaysCount,
            PortalSettingNames.UsdCurrencyId,
            ContractNotificationSettingNames.ContractExpirationFirstReminderInDays,
            ContractNotificationSettingNames.ContractExpirationSecondReminderInDays
        ];
    }

    protected override TeamBoardPortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new TeamBoardPortalSettingsDto
        {
            SystemAdministrationDepartmentId = Get(DepartmentSettingNames.SystemAdministrationDepartmentId).GetString(),
            SalesDepartmentId =  Get(DepartmentSettingNames.SalesDepartmentId).GetString(),
            OverdueDaysCount = Get(PerformanceReviewSettingNames.OverdueDaysCount).GetInt(),
            UpcomingIminentBorderDaysCount = Get(PerformanceReviewSettingNames.UpcomingIminentBorderDaysCount).GetInt(),
            UsdCurrencyId = Get(PortalSettingNames.UsdCurrencyId).GetString(),
            ContractExpirationFirstReminderInDays = Get(
                ContractNotificationSettingNames.ContractExpirationFirstReminderInDays).GetInt(),
            ContractExpirationSecondReminderInDays = Get(
                ContractNotificationSettingNames.ContractExpirationSecondReminderInDays).GetInt()
        };
    }
}
