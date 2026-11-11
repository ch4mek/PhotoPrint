using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;

namespace Modsen.Portal.TimeTrack.UseCases.TimeTrackPortalSettings;

public class TimeTrackPortalSettingsAccessor :
    BasePortalSettingsAccessor<TimeTrackPortalSettingsDto>,
    ITimeTrackPortalSettingsAccessor
{
    public TimeTrackPortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
        : base(portalSettingsAccessor) { }

    protected override string[] GetSettingNames()
    {
        return
        [
            PeopleForceSettingNames.BelarusLocationId,
            DepartmentSettingNames.SystemAdministrationDepartmentId,
            DepartmentSettingNames.PmoDepartmentId,
            DepartmentSettingNames.SalesDepartmentId,
            PortalSettingNames.DefaultTimesheetReportName,
            PortalSettingNames.ReportsAuthorName,
            PortalSettingNames.DefaultTimeReportFileName,

            ExcelSettingNames.SummaryReportWorksheet,
            ExcelSettingNames.SummaryWorkingHoursReportWorksheet,
            ExcelSettingNames.DefaultFontName,
            ExcelSettingNames.BaseFontSize,
            ExcelSettingNames.NumberFormat,
            EquipmentSettingNames.DefaultDateFormat
        ];
    }

    protected override TimeTrackPortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new TimeTrackPortalSettingsDto
        {
            SystemAdministrationDepartmentId = Get(DepartmentSettingNames.SystemAdministrationDepartmentId).GetString(),
            PmoDepartmentId = Get(DepartmentSettingNames.PmoDepartmentId).GetString(),
            SalesDepartmentId = Get(DepartmentSettingNames.SalesDepartmentId).GetString(),
            BelarusLocationId = Get(PeopleForceSettingNames.BelarusLocationId).GetInt(),
            DefaultTimesheetReportName = Get(PortalSettingNames.DefaultTimesheetReportName).GetString(),
            ReportsAuthorName = Get(PortalSettingNames.ReportsAuthorName).GetString(),
            DefaultTimeReportFileName = Get(PortalSettingNames.DefaultTimeReportFileName).GetString(),
            ExcelSettings = new TimeTrackPortalSettingsDto.Excel
            {
                SummaryReportWorksheet = Get(ExcelSettingNames.SummaryReportWorksheet).GetString(),
                SummaryWorkingHoursReportWorksheet = Get(ExcelSettingNames.SummaryWorkingHoursReportWorksheet).GetString(),
                DefaultFontName = Get(ExcelSettingNames.DefaultFontName).GetString(),
                BaseFontSize = Get(ExcelSettingNames.BaseFontSize).GetInt(),
                NumberFormat = Get(ExcelSettingNames.NumberFormat).GetString(),
                DefaultDateFormat = Get(EquipmentSettingNames.DefaultDateFormat).GetString()
            }
        };
    }
}
