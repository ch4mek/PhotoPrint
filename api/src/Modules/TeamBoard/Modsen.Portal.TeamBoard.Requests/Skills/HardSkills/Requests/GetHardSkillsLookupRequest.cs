using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

internal sealed class GetHardSkillsLookupRequest
    : IQueryHandler<GetHardSkillsLookupRequest.Query, List<HardSkillLookupDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetHardSkillsLookupRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<HardSkillLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
         return await _teamBoardDbContext.HardSkills
             .Where(hardSkill => hardSkill.IsActive)
             .Select(hardSkill => new HardSkillLookupDto(hardSkill.Id, hardSkill.Name))
             .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<HardSkillLookupDto>>;
}
