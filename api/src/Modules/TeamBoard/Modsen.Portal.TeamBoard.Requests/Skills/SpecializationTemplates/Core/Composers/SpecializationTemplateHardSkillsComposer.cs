using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Composers;

internal interface ISpecializationTemplateHardSkillsComposer
{
    Task<SpecializationTemplateHardSkillDto[]> ComposeAsync(int specializationTemplateId, string[] hardSkillsNames,
        CancellationToken cancellationToken);
}

internal sealed class SpecializationTemplateHardSkillsComposer : ISpecializationTemplateHardSkillsComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamboardDbContext;
    private readonly IClockService _clockService;

    public SpecializationTemplateHardSkillsComposer(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamboardDbContext,
        IClockService clockService)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamboardDbContext = teamboardDbContext;
        _clockService = clockService;
    }

    public async Task<SpecializationTemplateHardSkillDto[]> ComposeAsync(int specializationTemplateId,
        string[] hardSkillsNames,
        CancellationToken cancellationToken)
    {
        var skills = _teamboardDbContext.HardSkills
            .Where(hardSkill => hardSkill.IsActive)
            .Where(hardSkill => hardSkillsNames.Contains(hardSkill.Name));

        var hardSkills = skills.Select(hardSkill => new SpecializationTemplateHardSkill
        {
            HardSkillId = hardSkill.Id,
            HardSkill = hardSkill,
            SpecializationTemplateId = specializationTemplateId,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.GetRequiredUserId()
        }).ToList();

        await _teamboardDbContext.SpecializationTemplateHardSkills.AddRangeAsync(hardSkills, cancellationToken);
        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return hardSkills.Select(hardSkill =>
            new SpecializationTemplateHardSkillDto
            {
                Id = hardSkill.HardSkillId,
                Name = hardSkill.HardSkill!.Name
            }).ToArray();
    }
}
