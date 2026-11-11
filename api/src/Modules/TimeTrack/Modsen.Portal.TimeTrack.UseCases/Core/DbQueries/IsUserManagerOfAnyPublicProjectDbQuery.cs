using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;

internal interface IIsUserManagerOfAnyPublicProjectDbQuery
{
    Task<bool> ExecuteAsync(int? userId, CancellationToken cancellationToken);
}

internal sealed class IsUserManagerOfAnyPublicProjectDbQuery : IIsUserManagerOfAnyPublicProjectDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public IsUserManagerOfAnyPublicProjectDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExecuteAsync(int? userId, CancellationToken cancellationToken)
    {
        return _dbContext.Projects.AnyAsync(project => project.ManagerUserId == userId && project.IsPublic,
            cancellationToken);
    }
}
