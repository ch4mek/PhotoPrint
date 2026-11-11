using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class GetTimeTrackUserByIdWithDepartmentDbQuery : IGetTimeTrackUserByIdWithDepartmentDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetTimeTrackUserByIdWithDepartmentDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<GetTimeTrackUserByIdWithDepartmentDbQueryResponse> ExecuteAsync(int userId,
        CancellationToken cancellationToken)
    {
        var result = await _timeTrackDbContext.TimeTrackUsers
            .Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);

        return new GetTimeTrackUserByIdWithDepartmentDbQueryResponse(
            result?.UserId, result?.DepartmentId, result?.ResourceGroupId, result?.Position, result?.IsActive, result?.CreatedById);
    }
}
