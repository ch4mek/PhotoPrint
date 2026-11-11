using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectUsersBySalesManagerRequest
    : IQueryHandler<GetProjectUsersBySalesManagerRequest.Query, List<int>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetProjectUsersBySalesManagerRequest(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        var projectUsers = _timeTrackDbContext
            .ProjectUsers
            .Where(u => u.Project!.Client!.SalesManagerId == request.SalesManagerId)
            .Select(u => u.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return projectUsers;
    }

    internal sealed record Query(int SalesManagerId) : IQuery<List<int>>, ITimeTrackRequest;
}
