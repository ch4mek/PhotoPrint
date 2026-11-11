using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;

internal class TimesheetFilter
{
    public int? UserId { get; set; }
    public string? DepartmentId { get; set; }
    public int? ClientId { get; set; }
    public int? ProjectId { get; set; }
    public int? ResourceGroupId { get; set; }
    public TrackedTimeType? TrackedTimeType { get; set;}
}
