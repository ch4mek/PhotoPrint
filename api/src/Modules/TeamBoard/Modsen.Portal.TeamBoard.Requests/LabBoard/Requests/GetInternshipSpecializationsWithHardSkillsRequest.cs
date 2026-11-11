using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;

internal sealed class GetInternshipSpecializationsWithHardSkillsRequest :
    IQueryHandler<GetInternshipSpecializationsWithHardSkillsRequest.Query, List<SpecializationTemplateWithHardSkillsDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetInternshipSpecializationsWithHardSkillsRequest(
        ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<SpecializationTemplateWithHardSkillsDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var specializations = await _teamBoardDbContext.SpecializationTemplates
            .AsNoTracking()
            .Include(x => x.SpecializationTemplateHardSkills)
                .ThenInclude(x => x.HardSkill)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var result = specializations
            .Select(SpecializationTemplateWithHardSkillsDto.Of)
            .ToList();

        return result;
    }

    internal sealed record Query() : IQuery<List<SpecializationTemplateWithHardSkillsDto>>;
}
