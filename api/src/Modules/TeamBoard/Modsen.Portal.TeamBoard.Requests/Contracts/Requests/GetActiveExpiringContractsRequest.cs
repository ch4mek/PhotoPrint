using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal class GetActiveExpiringContractsRequest : IRequestHandler<GetActiveExpiringContractsRequest.Query, List<ActiveExpiringContractDto>>
{
    private readonly TeamBoardDbContext _dbContext;
    private readonly IClockService _clockService;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetActiveExpiringContractsRequest(TeamBoardDbContext dbContext, IClockService clockService, IAuthenticatedUserContext authenticatedUserContext)
    {
        _dbContext = dbContext;
        _clockService = clockService;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<ActiveExpiringContractDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var requestFromDate = _clockService.TodayUtc();
        var requestToDate = requestFromDate.AddMonths(2);

        var query = _dbContext.UserContracts
            .Include(c => c.User)
            .Where(c => c.ContractTo != null && c.ContractTo >= requestFromDate && c.ContractTo <= requestToDate &&
                c.IsActive && c.User != null && c.User.IsActive)
            .OrderBy(c => c.ContractTo)
            .AsQueryable();

        query = FilterByRolesAccess(query);
        query = FilterByDepartment(request, query);

        var result = await query
            .OrderBy(c => c.ContractTo)
            .Select(c => new ActiveExpiringContractDto(
                c.Id,
                c.User!.UserId,
                c.User!.LastNameRu == null && c.User!.FirstNameRu == null
                    ? c.User!.LastName + " " + c.User!.FirstName
                    : (c.User!.LastNameRu + " " + c.User!.FirstNameRu + " (" + c.User!.LastName + " " + c.User!.FirstName + ")").Trim(),
                c.ContractTo,
                c.User.Position
            ))
            .ToListAsync(cancellationToken: cancellationToken);

        return result;
    }

    private static IQueryable<UserContract> FilterByDepartment(Query request, IQueryable<UserContract> query)
    {
        if (request.DepartmentId is not null)
        {
            query = query.Where(x => x.User!.DepartmentId == request.DepartmentId);
        }

        return query;
    }

    private IQueryable<UserContract> FilterByRolesAccess(IQueryable<UserContract> query)
    {
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
        }

        return query;
    }

    internal sealed record Query(string? DepartmentId) : IRequest<List<ActiveExpiringContractDto>>;
}
