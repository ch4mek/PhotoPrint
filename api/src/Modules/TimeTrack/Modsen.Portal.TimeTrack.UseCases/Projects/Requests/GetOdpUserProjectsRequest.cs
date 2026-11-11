using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetOdpUserProjectsRequest
    : IQueryHandler<GetOdpUserProjectsRequest.Query, PageableListOfItems<OdpProjectUser>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetOdpUserProjectsRequest(IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<PageableListOfItems<OdpProjectUser>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;
        var minDate = DateOnly.FromDateTime(DateTime.MinValue);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var query = _timeTrackDbContext.ProjectUsers.Select(ProjectUser.MapProjectUser).AsQueryable();

        query = GetQueryWithAccessFilter(query);
        query = GetQueryWithProjection(query, minDate);

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<OdpProjectUser>()
            .ToListAsync(cancellationToken);

        result = result.Select(x =>
        {
            x.UserOnProjectFrom = x.UserOnProjectFrom == minDate.AddDays(1) ? minDate : x.UserOnProjectFrom;
            x.IsActive = today <= x.UserOnProjectFrom ||
                (today >= x.UserOnProjectFrom &&
                (x.UserOnProjectTo == null || today <= x.UserOnProjectTo));
            return x;
        }).ToList();

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpProjectUser>()
            .CountAsync(cancellationToken);

        return result.ToPageable(queryOptions, total);
    }

    private IQueryable<OdpProjectUser> GetQueryWithAccessFilter(IQueryable<OdpProjectUser> query)
    {
        var userRoles = _authenticatedUserContext.GetRoles();

        if (userRoles.IsHeadOfDepartment())
        {
            query = query.Where(pu => pu.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (userRoles.IsGroupManager())
        {
            query = query.Where(pu => pu.ResourceGroupId == _authenticatedUserContext.ResourceGroupId
                || pu.UserId == _authenticatedUserContext.UserId);
        }

        if (userRoles.IsEmployee() || userRoles.IsSystemAdministrator())
        {
            query = query.Where(pu => pu.UserId == _authenticatedUserContext.UserId);
        }

        return query;
    }

    private IQueryable<OdpProjectUser> GetQueryWithProjection(IQueryable<OdpProjectUser> query, DateOnly minDate)
    {
        var isUserEmployeeOrSysAdmin = _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator();

        return query.Select(pu => new OdpProjectUser
        {
            Id = pu.Id,
            UserId = pu.UserId,
            ProjectId = pu.ProjectId,
            FirstName = pu.FirstName,
            LastName = pu.LastName,
            DepartmentId = pu.DepartmentId,
            ProjectName = pu.ProjectName,
            UserFullName = pu.UserFullName,
            UserOnProjectFrom = pu.UserOnProjectFrom <= minDate
                ? new DateOnly(0001, 1, 1).AddDays(1)
                : pu.UserOnProjectFrom,
            UserOnProjectTo = pu.UserOnProjectTo,
            ResourceGroupId = pu.ResourceGroupId,
            IsActive = pu.IsActive,
            IsTimeOnProjectBillable = isUserEmployeeOrSysAdmin
                ? null
                : pu.IsTimeOnProjectBillable,
            ClientName = isUserEmployeeOrSysAdmin
                ? string.Empty
                : pu.ClientName,
        });
    }

    internal sealed record Query(ODataQueryOptions<OdpProjectUser> QueryOptions) : IQuery<PageableListOfItems<OdpProjectUser>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
