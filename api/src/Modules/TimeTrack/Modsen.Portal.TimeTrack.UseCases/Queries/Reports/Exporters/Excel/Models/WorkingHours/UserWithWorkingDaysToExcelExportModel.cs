using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.WorkingHours;

public class UserWithWorkingDaysToExcelExportModel
{
    public int EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? DepartmentId { get; set; }
    public string? Position { get; set; }
    public IReadOnlyCollection<DayOfMonthToExcelExportModel> DaysOfMonth { get; set; } = [];

    internal static UserWithWorkingDaysToExcelExportModel? Of(UserWithWorkingDaysDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new UserWithWorkingDaysToExcelExportModel
        {
            EmployeeId = source.EmployeeId,
            FirstName = source.FirstName,
            LastName  = source.LastName,
            MiddleName = source.MiddleName,
            Email = source.Email,
            DepartmentId = source.DepartmentId,
            Position = source.Position,
            DaysOfMonth = source.DaysOfMonth.Select(DayOfMonthToExcelExportModel.Of).ToList()
        };
    }
}
