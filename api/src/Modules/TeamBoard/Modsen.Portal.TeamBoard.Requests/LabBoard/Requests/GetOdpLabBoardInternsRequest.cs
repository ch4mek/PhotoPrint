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

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;

internal sealed class GetOdpLabBoardInternsRequest
    : IQueryHandler<GetOdpLabBoardInternsRequest.Query, PageableListOfItems<OdpLabBoardInternDto>>
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IPageableOdpLabBoardInternToPageableOdpLabBoardInternDtoMapper _mapper;

    public GetOdpLabBoardInternsRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext dbContext,
        IPageableOdpLabBoardInternToPageableOdpLabBoardInternDtoMapper mapper)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PageableListOfItems<OdpLabBoardInternDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;
        var query = _dbContext.TeamBoardUsers
            .AsNoTracking()
            .Where(user => user.Position == PortalRoleNames.Intern)
            .AsSplitQuery()
            .Select(OdpLabBoardIntern.MapTeamBoardUser);

        query = GetFilteredQuery(query);

        var queryResult = await queryOptions
            .ApplyTo(query)
            .Cast<OdpLabBoardIntern>()
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpLabBoardIntern>()
            .CountAsync(cancellationToken);

        var pageableOdpLabBoardIntern = queryResult.ToPageable(queryOptions, totalCount);
        var pageableLabBoardInterns = _mapper.Map(pageableOdpLabBoardIntern);

        return pageableLabBoardInterns;
    }

    private IQueryable<OdpLabBoardIntern> GetFilteredQuery(IQueryable<OdpLabBoardIntern> query)
    {
        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            !_authenticatedUserContext.GetUserClaimsNames().Contains(LabBoardClaimTypeNames.ManageAllInterns.Name))
        {
            query = query.Where(user => user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        return query;
    }

    internal sealed record Query(ODataQueryOptions<OdpLabBoardIntern> QueryOptions)
        : IQuery<PageableListOfItems<OdpLabBoardInternDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
