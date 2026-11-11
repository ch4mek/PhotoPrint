using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Core.DbQueries;

internal interface IGetJiraSettingDbQuery
{
    Task<JiraSetting?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal sealed class GetJiraSettingDbQuery : IGetJiraSettingDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetJiraSettingDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<JiraSetting?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.JiraSettings
            .Where(x => x.IsActive)
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
