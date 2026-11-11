using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;

internal interface IIsHardSkillNameUniqueBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int? id, string name, CancellationToken cancellationToken);
}

internal sealed class IsHardSkillNameUniqueBusinessRule : IIsHardSkillNameUniqueBusinessRule
{
    private readonly ITeamBoardDbContext _dbContext;

    public IsHardSkillNameUniqueBusinessRule(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int? id, string name, CancellationToken cancellationToken)
    {
        return !await _dbContext.HardSkills.AnyAsync(hardSkill =>
                EF.Functions.Like(hardSkill.Name, name) &&
                id != hardSkill.Id &&
                hardSkill.IsActive,
            cancellationToken);
    }
}
