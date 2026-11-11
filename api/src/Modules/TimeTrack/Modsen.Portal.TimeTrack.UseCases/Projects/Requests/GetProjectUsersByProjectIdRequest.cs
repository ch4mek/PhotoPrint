using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectUsersByProjectIdRequest : IQueryHandler<GetProjectUsersByProjectIdRequest.Query, List<int>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IClockService _clockService;

    public GetProjectUsersByProjectIdRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IClockService clockService)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _clockService = clockService;
    }

    public async Task<List<int>> Handle(
        Query request, CancellationToken cancellationToken)
    {
        var today = _clockService.TodayUtc();

        var projectUsers = await _timeTrackDbContext.ProjectUsers
            .Where(x => x.ProjectId == request.Id)
            .Where(x => today <= x.UserOnProjectFrom ||
                today >= x.UserOnProjectFrom && (x.UserOnProjectTo == null || today <= x.UserOnProjectTo))
            .Select(x =>  x.UserId)
            .ToListAsync(cancellationToken);

        return projectUsers;
    }

    internal sealed record Query(int Id) : IQuery<List<int>>, ITimeTrackRequest;
}
