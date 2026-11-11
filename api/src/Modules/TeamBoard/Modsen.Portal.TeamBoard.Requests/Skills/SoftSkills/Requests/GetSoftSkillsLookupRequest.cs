using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;

internal sealed class GetSoftSkillsLookupRequest : IQueryHandler<GetSoftSkillsLookupRequest.Query, List<SoftSkillDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetSoftSkillsLookupRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;

    }

    public async Task<List<SoftSkillDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _teamBoardDbContext.SoftSkills
            .Where(skill => skill.IsActive)
            .Select(skill => new SoftSkillDto { Id = skill.Id, Name = skill.Name })
            .ToListAsync(cancellationToken);

        return result;
    }

    internal sealed record Query : IQuery<List<SoftSkillDto>>;
}
