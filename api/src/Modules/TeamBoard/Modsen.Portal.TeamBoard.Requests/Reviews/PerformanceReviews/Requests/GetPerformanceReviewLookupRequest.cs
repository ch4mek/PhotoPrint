using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests;

internal sealed class GetPerformanceReviewLookupRequest : IQueryHandler<GetPerformanceReviewLookupRequest.Query, PerformanceReviewLookupDto[]>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IPerformanceReviewDateTypeCalculator _performanceReviewDateTypeCalculator;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;
    private readonly IClockService _clockService;

    public GetPerformanceReviewLookupRequest(ITeamBoardDbContext teamBoardDbContext,
        IPerformanceReviewDateTypeCalculator performanceReviewDateTypeCalculator,
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor,
        IClockService clockService)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _performanceReviewDateTypeCalculator = performanceReviewDateTypeCalculator;
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _clockService = clockService;
    }

    public async Task<PerformanceReviewLookupDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var authRoles = _authenticatedUserContext.GetRoles();

        if (!authRoles.IsCto() && !authRoles.IsCeo()
            && !string.IsNullOrWhiteSpace(request.DepartmentId))
        {
            throw new ActionNotAllowedException("Filter parameter is not allowed for current position");
        }

        if (authRoles.IsEmployee() || authRoles.IsSystemAdministrator() || authRoles.IsIntern())
        {
            return [];
        }

        var upperDateBorder = _clockService.TodayStartOfDayUtc().AddDays(request.UpperDaysBorder);

        var query = _teamBoardDbContext.UserPerformanceReviews
            .AsNoTracking()
            .Where(x => x.User!.IsActive)
            .Where(x => x.User!.Position != PortalRoleNames.Intern)
            .Where(x => x.Status != UserPerformanceReviewStatus.Passed)
            .Where(x => x.InterviewDateFromUtc <= upperDateBorder);

        if (!string.IsNullOrWhiteSpace(request.DepartmentId))
        {
            query = query.Where(x => x.User!.DepartmentId == request.DepartmentId);
        }
        else if (authRoles.IsHeadOfDepartment())
        {
            query = query.Where(x => x.User!.DepartmentId == _authenticatedUserContext.DepartmentId);
        }
        else if (authRoles.IsGroupManager())
        {
            query = query.Where(x => x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        var performanceReviews = await query
            .OrderBy(x => x.InterviewDateFromUtc)
            .Select(PerformanceReviewLookupDto.Map)
            .ToArrayAsync(cancellationToken);

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);

        foreach (var performanceReview in performanceReviews)
        {
            performanceReview.PerformanceReviewDateType = _performanceReviewDateTypeCalculator
                .Calculate(performanceReview.Date, request.OverdueDays, settings.UpcomingIminentBorderDaysCount);
        }

        return performanceReviews;
    }

    public sealed record Query(int OverdueDays, int UpperDaysBorder, string? DepartmentId)
        : IQuery<PerformanceReviewLookupDto[]>;
}
