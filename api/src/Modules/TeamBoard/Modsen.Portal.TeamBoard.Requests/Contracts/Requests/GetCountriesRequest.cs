using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetCountriesRequest : IQueryHandler<GetCountriesRequest.Query, List<CountryDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetCountriesRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;

    }

    public Task<List<CountryDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.Countries
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new CountryDto{
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<CountryDto>>;
}
