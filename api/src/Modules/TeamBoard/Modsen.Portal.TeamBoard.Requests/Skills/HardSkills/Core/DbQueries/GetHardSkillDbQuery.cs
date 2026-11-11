using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.DbQueries;

internal interface IGetHardSkillDbQuery
{
    Task<HardSkill?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal sealed class GetHardSkillDbQuery : IGetHardSkillDbQuery
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetHardSkillDbQuery(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<HardSkill?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.HardSkills
            .FirstOrDefaultAsync(skill => skill.Id == id, cancellationToken);
    }
}
