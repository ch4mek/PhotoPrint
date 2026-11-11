using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

public class DashboardUsersCoreDto
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public IReadOnlyCollection<DashboardUser> Items { get; set; } = [];
}
