using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Core.Composers;

internal interface IProjectUserComposer
{
    Task ComposeAsync(CreateProjectUserCoreDto request, CancellationToken cancellationToken);
}

internal class ProjectUserComposer : IProjectUserComposer
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public ProjectUserComposer(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task ComposeAsync(CreateProjectUserCoreDto request, CancellationToken cancellationToken)
    {
        var projectUser = request.ToDatabaseEntity();

        await _teamBoardDbContext.ProjectUsers.AddAsync(projectUser, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }
}
