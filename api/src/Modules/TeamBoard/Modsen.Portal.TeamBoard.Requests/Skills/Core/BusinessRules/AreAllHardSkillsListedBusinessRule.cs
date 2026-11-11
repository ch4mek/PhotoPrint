using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;

internal interface IAreAllHardSkillsListedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int specializationTemplateId, IReadOnlyCollection<HardSkillSeniorityDto> hardSkillsToFind,
        CancellationToken cancellationToken);
}

internal class AreAllHardSkillsListedBusinessRule : IAreAllHardSkillsListedBusinessRule
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public AreAllHardSkillsListedBusinessRule(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(
        int specializationTemplateId,
        IReadOnlyCollection<HardSkillSeniorityDto> hardSkillsToFind,
        CancellationToken cancellationToken)
    {
        var specializationTemplateInfo = await _teamBoardDbContext.SpecializationTemplates
            .Where(template => template.Id == specializationTemplateId)
            .Select(x => new
            {
                SpecializationTemplateId = x.Id,
                ExistingHardSkillsIds = x.SpecializationTemplateHardSkills
                    .Select(specializationTemplateHardSkill => specializationTemplateHardSkill.HardSkillId)
                    .Distinct()
                    .ToArray()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (specializationTemplateInfo?.SpecializationTemplateId is null)
        {
            throw new EntityNotFoundException($"Specialization template with id {specializationTemplateId} was not found");
        }

        var expectedHardSkillsIds = hardSkillsToFind.Select(hs => hs.Id).ToHashSet();

        return expectedHardSkillsIds.Count == specializationTemplateInfo.ExistingHardSkillsIds.Length &&
            specializationTemplateInfo.ExistingHardSkillsIds.All(hardSkillId => expectedHardSkillsIds.Contains(hardSkillId));
    }
}
