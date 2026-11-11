using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;

internal interface IDeleteSpecializationTemplateHardSkillsDbQuery
{
    Task<int[]> ExecuteAsync(
        int specializationTemplateId,
        IReadOnlyCollection<int>? hardSkillsIds,
        CancellationToken cancellationToken);
}

internal sealed class DeleteSpecializationTemplateHardSkillsDbQuery : IDeleteSpecializationTemplateHardSkillsDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public DeleteSpecializationTemplateHardSkillsDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<int[]> ExecuteAsync(
        int specializationTemplateId,
        IReadOnlyCollection<int>? hardSkillsIds,
        CancellationToken cancellationToken)
    {
        var templates = _teamBoardDbContext.SpecializationTemplateHardSkills
            .AsNoTracking();

        templates = hardSkillsIds is null
            ? templates.Where(hardSkill => hardSkill.SpecializationTemplateId == specializationTemplateId)
            : templates.Where(hardSkill => hardSkill.SpecializationTemplateId == specializationTemplateId &&
                hardSkillsIds.Contains(hardSkill.HardSkillId));

        _teamBoardDbContext.SpecializationTemplateHardSkills.RemoveRange(templates);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return await templates
            .Select(hardSkill => hardSkill.HardSkillId)
            .ToArrayAsync(cancellationToken);
    }
}
