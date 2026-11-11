using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetTrackedTasksByUserAndProjectRequest : IQueryHandler<GetTrackedTasksByUserAndProjectRequest.Query, List<TrackedTaskReportDto>>
{
    private readonly IUserAccessService _userAccessService;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IGetTimesheetsByPeriodWithFilterDbQuery _getTimesheetsDbQuery;

    public GetTrackedTasksByUserAndProjectRequest(
        IUserAccessService userAccessService,
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetTimesheetsByPeriodWithFilterDbQuery getTimesheetsByPeriodWithFilterDbQuery)
    {
        _userAccessService = userAccessService;
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _getTimesheetsDbQuery = getTimesheetsByPeriodWithFilterDbQuery;
    }

    public async Task<List<TrackedTaskReportDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!await _userAccessService.CanAccessUserReportInformation(request.UserId, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.CannotAccessOtherUserData);
        }

        var user = await _timeTrackDbContext.TimeTrackUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        var filter = new TimesheetFilter
        {
            UserId = request.UserId,
            ProjectId = request.ProjectId,
            DepartmentId = request.DepartmentId,
            ClientId = request.ClientId,
            ResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager()
                ? _authenticatedUserContext.ResourceGroupId
                : null,
            TrackedTimeType = request.TrackedTimeType
        };

        var dbRequest = new GetTimesheetsByPeriodWithFilterDbRequest
            (request.Year, request.MonthOfYear, request.WeekOfYear, filter);

        var timesheets =await _getTimesheetsDbQuery.ExecuteAsync(dbRequest, cancellationToken);

        var a = timesheets
            .Select(x => new TrackedTaskReportDto
            {
                ReportForDate = x.ReportForDate,
                TaskName = x.TaskName,
                TrackedTimeInMinutes = request.TrackedTimeType switch
                {
                    TrackedTimeType.Fact => x.FactTrackedInMinutes,
                    TrackedTimeType.Billable => x.BillableTrackedInMinutes,
                    _ => throw new InvalidOperationException("Unsupported tracked time type")
                },
                Color = user.Color
            })
            .OrderBy(t => t.ReportForDate)
            .ToList();

        return a;
    }

    internal sealed record Query(
            int? Year,
            int[]? MonthOfYear,
            int? WeekOfYear,
            int UserId,
            int ProjectId,
            string? DepartmentId,
            int? ClientId,
            TrackedTimeType TrackedTimeType)
        : IQuery<List<TrackedTaskReportDto>>, IPeriodRequest, ITimeTrackRequest;
}
