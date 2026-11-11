using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetUserContractsDatesRequest : IQueryHandler<GetUserContractsDatesRequest.Query, List<UserContractDatesDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetUserContractsDatesRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<UserContractDatesDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _teamBoardDbContext.UserContracts
            .Where(contract => contract.UserId == request.userId);

        if (request.OnlyActiveUsers)
        {
            query = query.Where(contract => contract.IsActive);
        }

        return await query
            .Include(contract => contract.User)
            .Select(contract => new UserContractDatesDto
            {
                UserId = contract.UserId,
                ContractStartDate = contract.ContractFrom,
                ContractEndDate = contract.ContractTo,
                HireDate = contract.User!.HireDate,
                FireDate = contract.FireDate,
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int userId, bool OnlyActiveUsers) : IQuery<List<UserContractDatesDto>>;
}
