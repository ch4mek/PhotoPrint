using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetDashboardCurrentUserRequest
    : IQueryHandler<GetDashboardCurrentUserRequest.Query, DashboardCurrentUserCoreDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;
    private readonly IDashboardOdpLoader _dashboardOdpLoader;
    public GetDashboardCurrentUserRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        ITimesheetQueryBuilder timesheetQueryBuilder,
        IDashboardOdpLoader dashboardOdpLoader)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _timesheetQueryBuilder = timesheetQueryBuilder;
        _dashboardOdpLoader = dashboardOdpLoader;
    }

    public async Task<DashboardCurrentUserCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserContext.GetRequiredUserId();
        var currentDateTime = _clockService.UtcNow;
        var date = DateOnly.FromDateTime(currentDateTime);
        var week = date.GetWeekOfYear();
        var year = week is 1 && date.Month is not 1 ? date.Year + 1 : date.Year;

        var result = await _dashboardOdpLoader.LoadAsync(userId, date, request.QueryOptions, cancellationToken);

        var trackedInWeek = await _timesheetQueryBuilder
            .FromAsNoTracked(year: year, monthOfYear: null)
            .WithWeekOfYear(week)
            .WithUserFilter(userId)
            .Build()
            .SumAsync(t => t.FactTrackedInMinutes, cancellationToken);

        return new DashboardCurrentUserCoreDto(
            result.AllItems.Where(x => x.ReportForDate.Day == currentDateTime.Day).Sum(x => x.TrackedInMinutes),
            trackedInWeek,
            result.AllItems.Where(x => x.ReportForDate.Month == currentDateTime.Month).Sum(x => x.TrackedInMinutes),
            result.Pages);
    }

    internal sealed record Query(ODataQueryOptions<DashboardActivity> QueryOptions)
        : IQuery<DashboardCurrentUserCoreDto>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
