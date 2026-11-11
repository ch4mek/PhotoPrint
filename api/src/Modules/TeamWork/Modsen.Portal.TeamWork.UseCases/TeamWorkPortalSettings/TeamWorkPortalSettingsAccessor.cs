using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;

namespace Modsen.Portal.TeamWork.UseCases.TeamWorkPortalSettings;

internal interface ITeamWorkPortalSettingsAccessor
{
    Task<TeamWorkPortalSettingsDto> GetSettings(CancellationToken token);
}

internal class TeamWorkPortalSettingsAccessor :
    BasePortalSettingsAccessor<TeamWorkPortalSettingsDto>,
    ITeamWorkPortalSettingsAccessor
{
    public TeamWorkPortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
        : base(portalSettingsAccessor) {}

    protected override TeamWorkPortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new TeamWorkPortalSettingsDto
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
