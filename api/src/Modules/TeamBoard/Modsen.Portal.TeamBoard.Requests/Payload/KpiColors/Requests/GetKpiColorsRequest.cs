using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests;

internal sealed class GetKpiColorsRequest : IQueryHandler<GetKpiColorsRequest.Query, List<KpiColorDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetKpiColorsRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<KpiColorDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var colors = await _teamBoardDbContext.KpiColors.ToListAsync(cancellationToken);

        return colors.Select(color => new KpiColorDto
            {
                Id = color.Id,
                Name = color.Name,
                RightIntervalBorder = color.RightIntervalBorder,
            }).ToList();
    }

    internal sealed record Query : IQuery<List<KpiColorDto>>;
}
