using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IGetProjectUserByIdDbQuery
{
    Task<ProjectUserCoreDto?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal class GetProjectUserByIdDbQuery : IGetProjectUserByIdDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetProjectUserByIdDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<ProjectUserCoreDto?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        var projectUser = await _teamBoardDbContext.ProjectUsers
            .Include(user => user.Project)
            .Include(user => user.User)
            .Include(user => user.UserOnProjectSeniorityLevel)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        return ProjectUserCoreDto.Of(projectUser);
    }
}
