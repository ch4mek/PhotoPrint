using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

internal interface IGetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery
    : IDbQuery<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbRequest, List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>>;
    
internal record GetTimesheetByProjectIdAndUserIdAndDateIntervalDbRequest(int ProjectId, int UserId, DateOnly DateFrom,
    DateOnly? DateTo);
    
internal record GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse(int ProjectId, int UserId, DateOnly ReportForDate);
