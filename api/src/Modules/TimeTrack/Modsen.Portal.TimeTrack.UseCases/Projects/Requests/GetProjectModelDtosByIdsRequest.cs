using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectModelDtosByIdsRequest : IQueryHandler<GetProjectModelDtosByIdsRequest.Query, List<ProjectModelDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetProjectModelDtosByIdsRequest(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<ProjectModelDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .AsNoTracking()
            .Where(project => request.Ids.Contains(project.Id))
            .Select(project => new ProjectModelDto
            {
                Id = project.Id,
                IsActive = project.IsActive,
                IsPublic = project.IsPublic
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(List<int> Ids) : IQuery<List<ProjectModelDto>>, ITimeTrackRequest;
}
