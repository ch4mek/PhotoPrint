using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Attendance.Requests.Attendances.Requests;

internal class GetDetailedAttendancesRequest :
    IQueryHandler<GetDetailedAttendancesRequest.Query, PageableListOfItems<OdpAttendance>>
{
    private readonly IAttendanceDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetDetailedAttendancesRequest(IAttendanceDbContext dbContext, IAuthenticatedUserContext authenticatedUserContext)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<OdpAttendance>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Attendances
            .AsNoTracking()
            .Select(OdpAttendance.Map);

        query = ApplyAuthenticatedContextFilter(query);

        var attendances = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpAttendance>()
            .ToListAsync(cancellationToken);

        var total = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .Cast<OdpAttendance>()
            .CountAsync(cancellationToken);

        return attendances.ToPageable(request.QueryOptions, total);
    }

    private IQueryable<OdpAttendance> ApplyAuthenticatedContextFilter(IQueryable<OdpAttendance> query)
    {
        var roles = _authenticatedUserContext.GetRoles();
        if (roles.IsHeadOfDepartment() || roles.IsGroupManager())
        {
            query = query.Where(x => x.UserDepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (roles.IsGroupManager())
        {
            query = query.Where(x => x.UserResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        return query;
    }

    internal record Query(ODataQueryOptions<OdpAttendance> QueryOptions) : IQuery<PageableListOfItems<OdpAttendance>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
