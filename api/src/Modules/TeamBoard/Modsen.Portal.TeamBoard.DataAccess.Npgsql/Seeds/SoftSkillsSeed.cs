using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class SoftSkillsSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly TeamBoardDbContext _dbContext;

    public SoftSkillsSeed(TeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        foreach (var skill in GetSoftSkills())
        {
            if (!_dbContext.SoftSkills.Any(x => x.Name != skill.Name))
            {
                _dbContext.SoftSkills.Add(skill);
            }
        }

        _dbContext.SaveChanges();
    }

    private static IEnumerable<SoftSkill> GetSoftSkills()
    {
        yield return Create("Educability");
        yield return Create("Mentoring");
        yield return Create("Teamwork");
        yield return Create("Work under conditions of uncertainty");
        yield return Create("Critical thinking");
        yield return Create("Project management");
        yield return Create("Communication");
        yield return Create("Client oriented");
        yield return Create("Decision making");

        static SoftSkill Create(string name)
        {
            return new SoftSkill
            {
                Name = name,
                IsActive = true,
                CreatedById = null,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }

}
