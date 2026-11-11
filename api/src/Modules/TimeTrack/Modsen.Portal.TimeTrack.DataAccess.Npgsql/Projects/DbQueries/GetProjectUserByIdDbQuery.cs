using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class GetProjectUserByIdDbQuery : IGetProjectUserByIdDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetProjectUserByIdDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<GetProjectUserByIdDbResponse?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        var projectUser = await _timeTrackDbContext.ProjectUsers
            .Include(x=>x.User)
            .Include(x=>x.Project)
            .ThenInclude(x=>x!.Client)
            .FirstOrDefaultAsync(x => x.Id == id,
            cancellationToken);
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        return projectUser == null
            ? null
            : new GetProjectUserByIdDbResponse(
                projectUser.Id,
                projectUser.UserId,
                projectUser.ProjectId,
                projectUser.UserOnProjectFrom,
                projectUser.UserOnProjectTo,
                projectUser.IsTimeOnProjectBillable,
                projectUser.Fte,
                projectUser.Project!.Name,
                projectUser.Project.Client!.Name,
                projectUser.User!.LastName + " "  + projectUser.User.FirstName,
                projectUser.UserOnProjectTo == null
                || (projectUser.UserOnProjectFrom <= date && date <= projectUser.UserOnProjectTo)
                || date <= projectUser.UserOnProjectFrom
            );
    }
}
