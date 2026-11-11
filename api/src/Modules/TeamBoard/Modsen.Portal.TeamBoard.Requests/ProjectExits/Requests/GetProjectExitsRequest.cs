using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;

internal sealed class
    GetProjectExitsRequest : IQueryHandler<GetProjectExitsRequest.Query, PageableListOfItems<OdpProjectExitDto>>
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetProjectExitsRequest(ITeamBoardDbContext dbContext, IAuthenticatedUserContext authenticatedUserContext)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<OdpProjectExitDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var baseQuery = _dbContext.ProjectExits.AsNoTracking();

        var query = baseQuery.Select(OdpProjectExit.Map);

        query = GetQueryWithAccessFilter(query);

        var totalCount = await queryOptions
            .RebuildWithoutPagination()
            .ApplyTo(query)
            .Cast<OdpProjectExit>()
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<OdpProjectExitDto>.Value;
        }

        var items = await queryOptions
            .ApplyTo(query)
            .Cast<OdpProjectExit>()
            .Select(OdpProjectExitDto.Map)
            .ToListAsync(cancellationToken);

        SetIsManagedByRoleRequest(items);

        return items.ToPageable(queryOptions, totalCount);
    }

    private void SetIsManagedByRoleRequest(List<OdpProjectExitDto> items)
    {
        var userRoles = _authenticatedUserContext.GetRoles();

        foreach (var item in items)
        {
            item.IsManagedByRole =
                userRoles.IsHeadOfDepartment() && item.DepartmentId == _authenticatedUserContext.DepartmentId
                || userRoles.IsGroupManager() && item.UserResourceGroupId == _authenticatedUserContext.ResourceGroupId;
        }
    }

    private IQueryable<OdpProjectExit> GetQueryWithAccessFilter(IQueryable<OdpProjectExit> query)
    {
        var canUserSeeAllExits = _authenticatedUserContext
            .HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name);

        if (!canUserSeeAllExits)
        {
            query = GetQueryAccordingToRoleFilter(query);
        }

        return query;
    }

    private IQueryable<OdpProjectExit> GetQueryAccordingToRoleFilter(IQueryable<OdpProjectExit> query)
    {
        var userRoles = _authenticatedUserContext.GetRoles();

        if (userRoles.IsHeadOfDepartment())
        {
            query = query.Where(pe => pe.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (userRoles.IsGroupManager())
        {
            query = query.Where(pu => pu.UserResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        if (userRoles.IsEmployee())
        {
            query = query.Where(_ => false);
        }

        return query;
    }

    internal sealed record Query(ODataQueryOptions<OdpProjectExit> QueryOptions)
        : IQuery<PageableListOfItems<OdpProjectExitDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
