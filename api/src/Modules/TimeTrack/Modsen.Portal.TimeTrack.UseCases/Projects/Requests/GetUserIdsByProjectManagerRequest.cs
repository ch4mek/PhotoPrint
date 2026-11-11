using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetUserIdsByProjectManagerRequest : IQueryHandler<GetUserIdsByProjectManagerRequest.Query, List<int>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IClockService _clockService;

    public GetUserIdsByProjectManagerRequest(ITimeTrackDbContext timeTrackDbContext, IClockService clockService)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _clockService = clockService;
    }

    public Task<List<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        var now = _clockService.TodayUtc();

        return _timeTrackDbContext.Projects
            .Where(p => p.ManagerUserId == request.ManagerId)
            .SelectMany(p => p.ProjectUsers)
            .Where(x =>
                x.UserOnProjectTo == null
                || (x.UserOnProjectFrom <= now && now <= x.UserOnProjectTo)
                || now <= x.UserOnProjectFrom)
            .Select(pu => pu.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int ManagerId) : IQuery<List<int>>, ITimeTrackRequest;
}
