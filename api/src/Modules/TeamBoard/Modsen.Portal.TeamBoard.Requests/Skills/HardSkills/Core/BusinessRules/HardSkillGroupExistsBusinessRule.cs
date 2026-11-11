using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;

internal interface IHardSkillGroupExistsBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(string groupId, CancellationToken cancellationToken);
}

internal class HardSkillGroupExistsBusinessRule : IHardSkillGroupExistsBusinessRule
{
    private readonly ITeamBoardDbContext _dbContext;

    public HardSkillGroupExistsBusinessRule(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> IsCorrespondsToBusiness(string groupId, CancellationToken cancellationToken)
    {
        return _dbContext.HardSkillGroups.AnyAsync(hardSkill => hardSkill.Id == groupId, cancellationToken);
    }
}
