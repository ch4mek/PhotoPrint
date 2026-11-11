using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;

namespace Modsen.Portal.TeamWork.UseCases;

internal sealed class GetProjectsTrackedTimeUseCase : IQueryHandler<GetProjectsTrackedTimeUseCase.Query, GetProjectsTrackedTimeUseCase.Response>
{
    private readonly ITimeTrackContract _timeTrackContract;

    public GetProjectsTrackedTimeUseCase(ITimeTrackContract timeTrackContract)
    {
        _timeTrackContract = timeTrackContract;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _timeTrackContract.GetSumTrackedTimeByProjectsIds(request.Ids, cancellationToken);
        return new Response(result);
    }

    internal sealed record Query(IReadOnlyCollection<int> Ids) : IQuery<Response>;
    internal record Response(IReadOnlyCollection<ProjectsTrakedTimeOutput> Projects);
}
