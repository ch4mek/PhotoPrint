using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetActiveUsersContractDatesRequest : IQueryHandler<GetActiveUsersContractDatesRequest.Query, List<UserContractDatesDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public GetActiveUsersContractDatesRequest(ITeamBoardDbContext teamBoardDbContext, IClockService clockService)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public async Task<List<UserContractDatesDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var date = request.Date ?? _clockService.TodayUtc();

        return await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.IsActive)
            .Select(user => new
            {
                User = user,
                ActiveUserContract = user.Contracts
                    .Where(contract => contract.ContractFrom <= date)
                    .Where(contract => contract.ContractTo >= date || contract.ContractTo == null)
                    .FirstOrDefault(contract => contract.FireDate >= date || contract.FireDate == null)
            })
            .Where(x => x.ActiveUserContract != null)
            .Select(userContract => new UserContractDatesDto
            {
                UserId = userContract.User.UserId,
                ContractStartDate = userContract.ActiveUserContract!.ContractFrom,
                ContractEndDate = userContract.ActiveUserContract.ContractTo,
                HireDate = userContract.User.HireDate,
                FireDate = userContract.ActiveUserContract.FireDate,
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(DateOnly? Date) : IQuery<List<UserContractDatesDto>>;
}
