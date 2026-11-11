using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Core.Composers;

internal interface ITeamBoardProjectComposer
{
    Task ComposeAsync(CreateProjectCoreDto request, CancellationToken cancellationToken);
}

internal class TeamBoardProjectComposer : ITeamBoardProjectComposer
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public TeamBoardProjectComposer(ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public async Task ComposeAsync(CreateProjectCoreDto request, CancellationToken cancellationToken)
    {
        var project = request.ToDatabaseEntity(updatedAtUtc: _clockService.UtcNow);

        await _teamBoardDbContext.TeamBoardProjects.AddAsync(project, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }
}
