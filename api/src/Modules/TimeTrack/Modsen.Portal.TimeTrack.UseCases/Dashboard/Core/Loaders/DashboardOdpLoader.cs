using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;

internal interface IDashboardOdpLoader
{
    Task<DashboardOdpCoreDto> LoadAsync(
        int userId,
        DateOnly date,
        ODataQueryOptions<DashboardActivity> queryOptions,
        CancellationToken cancellationToken);
}

internal class DashboardOdpLoader : IDashboardOdpLoader
{
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;

    public DashboardOdpLoader(ITimesheetQueryBuilder timesheetQueryBuilder)
    {
        _timesheetQueryBuilder = timesheetQueryBuilder;
    }

    public async Task<DashboardOdpCoreDto> LoadAsync(
        int userId,
        DateOnly date,
        ODataQueryOptions<DashboardActivity> queryOptions,
        CancellationToken cancellationToken)
    {
        var list = new List<DashboardActivity>();
        var query = _timesheetQueryBuilder
            .FromAsNoTracked(date.Year, [date.Month])
            .WithUserFilter(userId)
            .WithBillableHoursFilter(false)
            .Build()
            .Select(Timesheet.Map);

        var queryResult = await queryOptions.ApplyTo(query)
            .Cast<DashboardActivity>()
            .ToListAsync(cancellationToken);

        var queryOptionsWithoutPagination = queryOptions.RebuildWithoutPagination();

        var totalCount = await queryOptionsWithoutPagination
            .ApplyTo(query)
            .Cast<DashboardActivity>()
            .CountAsync(cancellationToken);
        if (totalCount != 0)
        {
            list = await query.ToListAsync(cancellationToken);
        }

        return new DashboardOdpCoreDto
        {
            Pages = queryResult.ToPageable(queryOptions, totalCount),
            AllItems = list,
        };
    }
}
