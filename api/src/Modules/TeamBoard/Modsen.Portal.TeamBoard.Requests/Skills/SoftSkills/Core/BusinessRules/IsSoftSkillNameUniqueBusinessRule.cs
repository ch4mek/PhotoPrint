using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.BusinessRules;

internal interface IIsSoftSkillNameUniqueBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int? id, string name, CancellationToken cancellationToken);
}

internal class IsSoftSkillNameUniqueBusinessRule : IIsSoftSkillNameUniqueBusinessRule
{
    private readonly ITeamBoardDbContext _dbContext;

    public IsSoftSkillNameUniqueBusinessRule(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int? id, string name, CancellationToken cancellationToken)
    {
        return !await _dbContext.SoftSkills
            .AnyAsync(softSkill =>
                    EF.Functions.Like(softSkill.Name, name) &&
                    id != softSkill.Id &&
                    softSkill.IsActive,
                cancellationToken);
    }
}
