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

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetOdpUserContractsRequest : IQueryHandler<GetOdpUserContractsRequest.Query, PageableListOfItems<OdpUserContract>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetOdpUserContractsRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<OdpUserContract>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var query = _teamBoardDbContext.UserContracts
            .AsNoTracking()
            .Include(x => x.User)
            .AsQueryable();

        if (!_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name)
            && !_authenticatedUserContext.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name))
        {
            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
            {
                query = query.Where(x => x.User!.DepartmentId == _authenticatedUserContext.DepartmentId);
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager())
            {
                query = query.Where(x => x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
            }

            if (_authenticatedUserContext.GetRoles().IsEmployee() || _authenticatedUserContext.GetRoles().IsSystemAdministrator())
            {
                query = query.Where(x => x.User!.UserId == _authenticatedUserContext.GetRequiredUserId());
            }
        }

        var minDate = DateOnly.FromDateTime(DateTime.MinValue);


        var mappedQuery = query.Select(OdpUserContract.Map);
        
        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(mappedQuery)
            .OfType<OdpUserContract>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpUserContract>.Value;
        }

        var result = await queryOptions
            .ApplyTo(mappedQuery)
            .Cast<OdpUserContract>()
            .ToListAsync(cancellationToken);

        var usersContracts = result.Where(x => x.UserId == _authenticatedUserContext.GetRequiredUserId());

        foreach (var contract in usersContracts)
        {
            contract.FireDate = null;
            contract.FireDescription = null;
            contract.FireReason = null;
            contract.FireType = null;
        }

        result = result.Select(x => new OdpUserContract
        {
            Id = x.Id,
            UserId = x.UserId,
            ContractTo = x.ContractTo,
            ContractFrom = x.ContractFrom == minDate.AddDays(1) ? minDate : x.ContractFrom,
            FireDate = x.FireDate,
            FireType = x.FireType,
            FireReason = x.FireReason,
            FireDescription = x.FireDescription
        }).ToList();

        return result.ToPageable(queryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<OdpUserContract> QueryOptions)
        : IQuery<PageableListOfItems<OdpUserContract>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
