using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;
using Modsen.Portal.TeamBoard.Requests.Skills.SeniorityLevels.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SeniorityLevels.Requests;

internal sealed class GetSeniorityLevelsRequest : IQueryHandler<GetSeniorityLevelsRequest.Query, List<SeniorityLevelDto>>
{
    private readonly ISeniorityLevelCache _cache;

    public GetSeniorityLevelsRequest(ISeniorityLevelCache cache)
    {
        _cache = cache;
    }

    public async Task<List<SeniorityLevelDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var scopeValue = (int)request.SeniorityLevelScope;

        var result = (await _cache.GetAllSeniorityLevels(cancellationToken))
            .Where(level => ((level.SeniorityLevelScope & scopeValue) == scopeValue)).ToList();

        return result.ConvertAll(seniority => new SeniorityLevelDto
        {
            Id = seniority.Id!,
            Name = seniority.Name!
        });
    }

    internal sealed record Query(SeniorityLevelScopes SeniorityLevelScope) : IQuery<List<SeniorityLevelDto>>;
}
