using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Clients;

internal class GetClientNamesFromProjectsUserIsAssignedToDbQuery : IGetClientNamesFromProjectsUserIsAssignedToDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetClientNamesFromProjectsUserIsAssignedToDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .Where(x => x.ProjectUsers.Any(projectUser => projectUser.UserId == userId))
            .Where(x => !x.IsPublic)
            .Select(x => new { x.Client!.Id, x.Client.Name })
            .Distinct()
            .OrderBy(x => x.Id)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
