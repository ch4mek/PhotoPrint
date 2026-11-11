using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetWithTotalTrackedTime;

public record GetClientsWithTotalTrackedTimeDbRequest(
    int? Year,
    IReadOnlyCollection<int> MonthOfYear,
    int? WeekOfYear,
    string? DepartmentId,
    int? ClientId,
    int? ProjectId,
    int? UserId,
    int? ResourceGroupId,
    TrackedTimeType TrackedTimeType);
