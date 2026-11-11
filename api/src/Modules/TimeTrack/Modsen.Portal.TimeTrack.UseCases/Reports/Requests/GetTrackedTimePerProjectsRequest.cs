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
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetTrackedTimePerProjectsRequest : IQueryHandler<GetTrackedTimePerProjectsRequest.Query, List<TrackedTimePerProjectReportDto>>
{
    private readonly IUserAccessService _userAccessService;
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery _getTimesheetsDbQuery;

    public GetTrackedTimePerProjectsRequest(
        IUserAccessService userAccessService,
        ITimeTrackDbContext timeTrackDbContext,
        IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery getTimesheetsDbQuery,
        IAuthenticatedUserContext authenticatedUser)
    {
        _userAccessService = userAccessService;
        _authenticatedUser = authenticatedUser;
        _timeTrackDbContext = timeTrackDbContext;
        _getTimesheetsDbQuery = getTimesheetsDbQuery;
    }

    public async Task<List<TrackedTimePerProjectReportDto>> Handle(
        Query request,
        CancellationToken cancellationToken)
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
            DepartmentId = request.DepartmentId,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
            ResourceGroupId = _authenticatedUser.GetRoles().IsGroupManager()
                    ? _authenticatedUser.ResourceGroupId
                    : null,
            TrackedTimeType = request.TrackedTimeType
        };

        var dbRequest = new GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest(
            request.Year,
            request.MonthOfYear,
            request.WeekOfYear,
            filter);

        var timesheets = await _getTimesheetsDbQuery.ExecuteAsync(dbRequest, cancellationToken);

        var trackedTimeSelector = request.TrackedTimeType.GetTrackedTimeSelector();

        return timesheets
            .GroupBy(x => new {
                x.ProjectId,
                ProjectName = x.Project!.Name,
                ClientName = x.Project.Client!.Name
            })
            .Select(g => new TrackedTimePerProjectReportDto
            {
                ProjectId = g.Key.ProjectId,
                ProjectName = g.Key.ProjectName,
                ClientName = g.Key.ClientName,
                TasksCount = g.Count(),
                TotalTimeInMinutes = g.Sum(trackedTimeSelector),
                UserIsActive = user.IsActive,
                Color = user.Color
            })
            .ToList();
    }

    internal sealed record Query(
            int UserId,
            int? Year,
            int[]? MonthOfYear,
            int? WeekOfYear,
            string? DepartmentId,
            int? ClientId,
            int? ProjectId,
            TrackedTimeType TrackedTimeType)
        : IQuery<List<TrackedTimePerProjectReportDto>>, IPeriodRequest, ITimeTrackRequest;
}
