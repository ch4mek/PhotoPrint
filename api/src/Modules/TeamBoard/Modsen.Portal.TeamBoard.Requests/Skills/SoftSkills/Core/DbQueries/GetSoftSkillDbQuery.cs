using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.DbQueries;

internal interface IGetSoftSkillDbQuery
{
    Task<SoftSkill?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal class GetSoftSkillDbQuery : IGetSoftSkillDbQuery
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetSoftSkillDbQuery(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SoftSkill?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.SoftSkills.FirstOrDefaultAsync(skill => skill.Id == id, cancellationToken);
    }
}
