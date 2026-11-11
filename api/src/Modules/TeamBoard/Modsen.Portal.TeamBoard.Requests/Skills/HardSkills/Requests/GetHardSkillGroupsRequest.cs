using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

internal sealed class GetHardSkillGroupsRequest
    : IQueryHandler<GetHardSkillGroupsRequest.Query, List<ShortHardSkillGroupDto>>
{
    private readonly IHardSkillGroupCache _cache;

    public GetHardSkillGroupsRequest(IHardSkillGroupCache cache)
    {
        _cache = cache;
    }

    public async Task<List<ShortHardSkillGroupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _cache.GetAllHardSkillGroups(cancellationToken);

        return result.ConvertAll(group => new ShortHardSkillGroupDto
        {
            Id = group.Id!,
            Name = group.Name!
        });
    }

    internal sealed record Query : IQuery<List<ShortHardSkillGroupDto>>;
}
