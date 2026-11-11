using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

public class WorkingHoursPerMonthsSheetOutput
{
    public required IReadOnlyCollection<WorkingHoursPerMonthOutput> HoursPerMonth { get; set; } = [];
    public required IReadOnlyCollection<WorkingHoursTotalOutput> HoursPerYear { get; set; } = [];

    internal static WorkingHoursPerMonthsSheetOutput Of(WorkingHoursPerMonthsSheetDto source)
    {
        return new WorkingHoursPerMonthsSheetOutput
        {
            HoursPerMonth = source.HoursPerMonth.ToList().ConvertAll(x => new WorkingHoursPerMonthOutput
            {
                Id = x.Id,
                Year = x.Year,
                MonthOfYear = x.MonthOfYear,
                CountryId = x.CountryId,
                Hours = x.Hours
            }),

            HoursPerYear = source.HoursPerYear.ToList().ConvertAll(x => new WorkingHoursTotalOutput
            {
                Year = x.Year,
                CountryId = x.CountryId,
                Hours = x.Hours
            })
        };
    }
}
