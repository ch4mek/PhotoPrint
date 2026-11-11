using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

internal class CalendarDto
{
    public List<Country> Countries { get; set; } = [];

    public List<CalendarWorkingHourDetailsCoreDto> CalendarWorkingHourDetails { get; set; } = [];
}
