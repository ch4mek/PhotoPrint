using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Loaders;

internal interface IPayloadCalendarWorkingHourDetailsLoader
{
    Task<CalendarDto> Load(int payloadYear, CancellationToken token);
}

internal class PayloadCalendarWorkingHourDetailsLoader : IPayloadCalendarWorkingHourDetailsLoader
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public PayloadCalendarWorkingHourDetailsLoader(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<CalendarDto> Load(int payloadYear, CancellationToken token)
    {
        var countries = await _teamBoardDbContext.Countries.ToListAsync(token);

        var details = await _teamBoardDbContext.WorkingHoursPerMonths
            .AsNoTracking()
            .Where(x => x.Year == payloadYear)
            .GroupBy(x => x.MonthOfYear)
            .Select(group => new CalendarWorkingHourDetailsCoreDto
            {
                MonthOfYear = group.Key,
                CountryHours = group
                    .Select(workingHours => new CalendarCountryHourDetailsCoreDto
                    {
                        CountryId = workingHours.CountryId,
                        Hours = workingHours.Hours
                    })
                    .ToList()
            })
            .ToListAsync(token);

        return new CalendarDto
        {
            Countries = countries,
            CalendarWorkingHourDetails = details,
        };
    }
}
