using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetUsersExpiredContractsDatesRequest : IQueryHandler<GetUsersExpiredContractsDatesRequest.Query, List<UserContractDatesDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetUsersExpiredContractsDatesRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public Task<List<UserContractDatesDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.UserContracts
            .Where(x => x.IsActive)
            .Include(x => x.User)
            .Select(x => new
            {
                x.UserId,
                x.ContractFrom,
                x.ContractTo,
                x.User!.HireDate,
                x.FireDate,
                ContractEndDate = x.FireDate ?? x.ContractTo
            })
            .Where(x => x.ContractEndDate.HasValue)
            .Where(x => x.ContractEndDate!.Value == request.Date)
            .Select(x => new UserContractDatesDto
            {
                UserId = x.UserId,
                ContractStartDate = x.ContractFrom,
                ContractEndDate = x.ContractTo,
                HireDate = x.HireDate,
                FireDate = x.FireDate,
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(DateOnly Date) : IQuery<List<UserContractDatesDto>>;
}
