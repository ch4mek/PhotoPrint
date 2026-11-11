using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetUserContractDatesRequest : IQueryHandler<GetUserContractDatesRequest.Query, UserContractDatesDto?>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetUserContractDatesRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public Task<UserContractDatesDto?> Handle(Query request, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == request.UserId)
            .Select(user => new UserContractDatesDto
            {
                ContractEndDate = user.Contracts
                    .OrderBy(contract => contract.ContractFrom)
                    .Select(contract => contract.ContractTo)
                    .Last(),
                ContractStartDate = user.Contracts
                    .OrderBy(contract => contract.ContractFrom)
                    .Select(contract => contract.ContractFrom)
                    .Last(),
                HireDate = user.HireDate,
                UserId = user.UserId,
                FireDate = user.Contracts
                    .OrderBy(contract => contract.ContractFrom)
                    .Select(contract => contract.FireDate)
                    .Last()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    internal sealed record Query(int UserId) : IQuery<UserContractDatesDto?>;
}
