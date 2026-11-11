using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.GetProjectsCountAndAnyPublicProjectsByIds;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetNonExistingAndPublicProjectsByIdsRequest
    : IQueryHandler<GetNonExistingAndPublicProjectsByIdsRequest.Query, GetNonExistingAndPublicProjectsByIdsDbQueryResponse>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetNonExistingAndPublicProjectsByIdsRequest(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<GetNonExistingAndPublicProjectsByIdsDbQueryResponse> Handle(Query request, CancellationToken cancellationToken)
    {
        var existingProjectsByIds = await _timeTrackDbContext.Projects
            .Where(x => request.Ids.Contains(x.Id))
            .Select(x => new { x.Id, x.IsPublic })
            .ToListAsync(cancellationToken);

        var nonExistingProjectsIds = request.Ids
            .Except(existingProjectsByIds.Select(x => x.Id))
            .ToList();

        var publicProjectsIds = existingProjectsByIds
            .Where(x => x.IsPublic)
            .Select(x => x.Id)
            .ToList();

        return new GetNonExistingAndPublicProjectsByIdsDbQueryResponse(nonExistingProjectsIds, publicProjectsIds);
    }

    internal sealed record Query(IReadOnlyCollection<int> Ids) : IQuery<GetNonExistingAndPublicProjectsByIdsDbQueryResponse>, ITimeTrackRequest;
}
