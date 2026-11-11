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
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests;

internal sealed class GetOdpTeamBoardUsersRequest
    : IQueryHandler<GetOdpTeamBoardUsersRequest.Query, PageableListOfItems<OdpTeamBoardUserDto>>
{
    private readonly IPageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper _mapper;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _dbContext;

    public GetOdpTeamBoardUsersRequest(IPageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper mapper,
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext dbContext)
    {
        _mapper = mapper;
        _authenticatedUserContext = authenticatedUserContext;
        _dbContext = dbContext;
    }

    public async Task<PageableListOfItems<OdpTeamBoardUserDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;
        var query = _dbContext.TeamBoardUsers
            .AsNoTracking()
            .Where(user =>
                user.Position != PortalRoleNames.Intern &&
                user.IsActive &&
                user.Position != PortalRoleNames.Ceo &&
                user.Position != PortalRoleNames.Cto)
            .AsSplitQuery()
            .Select(OdpTeamBoardUser.MapTeamBoardUser);

        query = GetFilteredQuery(query);

        var queryResult = await queryOptions
            .ApplyTo(query)
            .Cast<OdpTeamBoardUser>()
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpTeamBoardUser>()
            .CountAsync(cancellationToken);

        var pageableOdpTeamBoardUser = queryResult.ToPageable(queryOptions, totalCount);
        var pageableTeamBoardUsers = _mapper.Map(pageableOdpTeamBoardUser);

        return pageableTeamBoardUsers;
    }

    private IQueryable<OdpTeamBoardUser> GetFilteredQuery(IQueryable<OdpTeamBoardUser> query)
    {
        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(user => user.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(user => user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId
                || user.Id == _authenticatedUserContext.UserId);
        }

        return query;
    }

    internal sealed record Query(ODataQueryOptions<OdpTeamBoardUser> QueryOptions)
        : IQuery<PageableListOfItems<OdpTeamBoardUserDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
