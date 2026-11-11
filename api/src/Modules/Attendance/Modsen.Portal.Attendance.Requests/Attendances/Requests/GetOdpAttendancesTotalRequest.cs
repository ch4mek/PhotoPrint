using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Attendance.Requests.Attendances.Core.Calculators;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Attendance.Requests.Attendances.Requests;

internal sealed class GetOdpAttendancesTotalRequest
    : IQueryHandler<GetOdpAttendancesTotalRequest.Query, PageableListOfItems<OdpAttendanceTotal>>
{
    private readonly IAttendanceDbContext _attendanceDbContext;
    private readonly IAttendanceTotalCalculator _attendanceTotalCalculator;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetOdpAttendancesTotalRequest(IAttendanceDbContext attendanceDbContext,
        IAttendanceTotalCalculator attendanceTotalCalculator,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _attendanceDbContext = attendanceDbContext;
        _attendanceTotalCalculator = attendanceTotalCalculator;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<OdpAttendanceTotal>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _attendanceDbContext.Attendances
            .AsNoTracking()
            .Include(x => x.User)
                .ThenInclude(x => x!.ResourceGroup)
            .AsSplitQuery()
            .AsQueryable();

        query = ApplyAuthenticatedContextFilter(query);
        var attendances = await query.ToListAsync(cancellationToken);

        var attendancesTotal = _attendanceTotalCalculator.Calculate(attendances)
            .AsQueryable();

        var total = request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(attendancesTotal)
            .OfType<OdpAttendanceTotal>()
            .Count();

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpAttendanceTotal>.Value;
        }

        var result = request.QueryOptions
            .ApplyTo(attendancesTotal)
            .Cast<OdpAttendanceTotal>()
            .ToList();

        return result.ToPageable(request.QueryOptions, total);
    }

    private IQueryable<AttendanceEntity> ApplyAuthenticatedContextFilter(IQueryable<AttendanceEntity> query)
    {
        var roles = _authenticatedUserContext.GetRoles();
        if (roles.IsHeadOfDepartment() || roles.IsGroupManager())
        {
            query = query.Where(x => x.User!.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (roles.IsGroupManager())
        {
            query = query.Where(x => x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        return query;
    }

    internal sealed record Query(ODataQueryOptions<OdpAttendanceTotal> QueryOptions)
        : IQuery<PageableListOfItems<OdpAttendanceTotal>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
