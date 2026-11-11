using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

public record DashboardCurrentUserCoreDto(
    int TrackedInMinutesDay, int TrackedInMinutesWeek, int TrackedInMinutesMonth,
    PageableListOfItems<DashboardActivity> Activities);
