using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

internal class DashboardOdpCoreDto
{
    public PageableListOfItems<DashboardActivity> Pages { get; set; } = EmptyPageableListOfItems<DashboardActivity>.Value;
    public IReadOnlyList<DashboardActivity> AllItems { get; set; } = [];
}
