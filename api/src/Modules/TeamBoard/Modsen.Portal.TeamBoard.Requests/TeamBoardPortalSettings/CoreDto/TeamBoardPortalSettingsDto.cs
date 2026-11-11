namespace Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings.CoreDto;

public class TeamBoardPortalSettingsDto
{
    public string? SystemAdministrationDepartmentId { get; init; }
    public string? SalesDepartmentId { get; init; }
    public int OverdueDaysCount { get; init; }
    public int UpcomingIminentBorderDaysCount { get; init; }
    public string? UsdCurrencyId { get; init; }
    public int ContractExpirationFirstReminderInDays { get; init; }
    public int ContractExpirationSecondReminderInDays { get; init; }
}
