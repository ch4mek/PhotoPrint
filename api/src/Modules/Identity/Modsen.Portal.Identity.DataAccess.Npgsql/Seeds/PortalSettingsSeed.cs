using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class PortalSettingsSeed(
    IdentityModuleDbContext dbContext,
    IHostEnvironmentContext hostEnvironment)
    : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    public void Seed()
    {
        var settings = new List<PortalSetting>
        {
            new() { Name = DatabaseGuardSettingNames.MaxCeoCount, Value = "1", ValueType = AllowedValueType.Integer },
            new() { Name = DatabaseGuardSettingNames.MaxCtoCount, Value = "1", ValueType = AllowedValueType.Integer },
            new() { Name = DatabaseGuardSettingNames.MaxHeadOfSpecificDepartmentCount, Value = "1", ValueType = AllowedValueType.Integer },
            new() { Name = DatabaseGuardSettingNames.MaxGmOfSpecificGroupCount, Value = "1", ValueType = AllowedValueType.Integer },

            new() { Name = PortalSettingNames.DefaultColorHex, Value = "#BDBDBD", ValueType = AllowedValueType.HexColor },
            new() { Name = PortalSettingNames.EmailDomain, Value = "@modsen-software.com", ValueType = AllowedValueType.String },
            new() { Name = PortalSettingNames.DefaultTimeZone, Value = "BY", ValueType = AllowedValueType.String },
            new() { Name = BookingSettingNames.ReservationsMonthCount, Value = "3",ValueType = AllowedValueType.Integer },
            new() { Name = PeopleForceSettingNames.BelarusLocationId, Value = "6446", ValueType = AllowedValueType.Integer },
            new() { Name = DepartmentSettingNames.SystemAdministrationDepartmentId, Value = "SysAdmin", ValueType = AllowedValueType.DepartmentId },
            new() { Name = DepartmentSettingNames.PmoDepartmentId, Value = "PMO", ValueType = AllowedValueType.DepartmentId },
            new() { Name = DepartmentSettingNames.SalesDepartmentId, Value = "Sales", ValueType = AllowedValueType.DepartmentId },
            new() { Name = PerformanceReviewSettingNames.OverdueDaysCount, Value = "7", ValueType = AllowedValueType.Integer },
            new() { Name = PerformanceReviewSettingNames.UpcomingIminentBorderDaysCount, Value = "3", ValueType = AllowedValueType.Integer },
            new() { Name = PortalSettingNames.UsdCurrencyId, Value = "USD", ValueType = AllowedValueType.CurrencyId },
            new() { Name = PortalSettingNames.MinskTimezoneHours, Value = "3", ValueType = AllowedValueType.Integer },
            new() { Name = PortalSettingNames.DefaultTimesheetReportName, Value = "TimeReportSummary", ValueType = AllowedValueType.String },
            new() { Name = PortalSettingNames.ReportsAuthorName, Value = "Modsen Company", ValueType = AllowedValueType.String },
            new() { Name = PortalSettingNames.DefaultTimeReportFileName, Value = "TimeReportSummary", ValueType = AllowedValueType.String },
            new() { Name = PortalSettingNames.MonthDayYearFormat, Value = "MM.dd.yyyy", ValueType = AllowedValueType.String },
            new() { Name = PortalSettingNames.MonthYearFormat, Value = "MMM yyyy", ValueType = AllowedValueType.String },
            new() { Name = PortalSettingNames.DefaultCountryId, Value = "1", ValueType = AllowedValueType.Integer },
            new() { Name = PortalSettingNames.DefaultRegisteredLocationId, Value = "1", ValueType = AllowedValueType.Integer },

            new() { Name = ExcelSettingNames.SummaryReportWorksheet, Value = "Summary Report", ValueType = AllowedValueType.String },
            new() { Name = ExcelSettingNames.SummaryWorkingHoursReportWorksheet, Value = "Working hours", ValueType = AllowedValueType.String },
            new() { Name = ExcelSettingNames.DefaultFontName, Value = "Calibri", ValueType = AllowedValueType.String },
            new() { Name = ExcelSettingNames.BaseFontSize, Value = "11", ValueType = AllowedValueType.Integer },
            new() { Name = ExcelSettingNames.NumberFormat, Value = "0.00", ValueType = AllowedValueType.String },
            new() { Name = ExcelSettingNames.EquipmentsFileName, Value = "Equipment", ValueType = AllowedValueType.String },
            new() { Name = ExcelSettingNames.DefaultColumnWidthInSymbols, Value = "30", ValueType = AllowedValueType.Integer },

            new() { Name = EquipmentSettingNames.FileExportDateFormat, Value = "dd.MM.yyyy", ValueType = AllowedValueType.String },
            new() { Name = EquipmentSettingNames.DefaultDateFormat, Value = "dd/MM/yyyy", ValueType = AllowedValueType.String },

            new() { Name = ContractNotificationSettingNames.ContractExpirationFirstReminderInDays, Value = "90", ValueType = AllowedValueType.Integer},
            new() { Name = ContractNotificationSettingNames.ContractExpirationSecondReminderInDays, Value = "45", ValueType = AllowedValueType.Integer},
            new() { Name = PortalSettingNames.DigitalHrContextChangeTimeInMinutes, Value = hostEnvironment.IsQaEnvironment() ? "10" : "240", ValueType = AllowedValueType.Integer },
            new() { Name = PortalSettingNames.EmployeesProbationPeriodEndDaysSpan, Value = "14", ValueType = AllowedValueType.Integer },
            new() { Name = PortalSettingNames.EmployeesEventsDaysSpan, Value = "7", ValueType = AllowedValueType.Integer },
        };

        var existingSettings = dbContext.PortalSettings.ToArray();

        foreach (var setting in settings)
        {
            var existingSetting = existingSettings.FirstOrDefault(x => x.Name == setting.Name);
            if (existingSetting == null)
            {
                dbContext.PortalSettings.Add(new PortalSetting
                {
                    Name = setting.Name,
                    Value = setting.Value,
                    ValueType = setting.ValueType
                });
            }
        }

        dbContext.SaveChanges();
    }
}
