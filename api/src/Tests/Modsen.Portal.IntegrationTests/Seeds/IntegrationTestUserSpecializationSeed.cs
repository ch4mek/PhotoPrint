using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.IntegrationTests.Seeds;

internal class IntegrationTestUserSpecializationSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly TeamBoardDbContext _dbContext;

    public IntegrationTestUserSpecializationSeed(TeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var supportUser = _dbContext.TeamBoardUsers.First(x => x.Email == "support@modsen-software.com");
        var specializationOne = new SpecializationTemplate
        {
            Name = ".NET Backend Engineer",
            IsActive = true,
            CreatedById = supportUser.UserId,
            CreatedAtUtc = DateTime.UtcNow
        };
        specializationOne.SpecializationTemplateHardSkills = _dbContext.HardSkills
            .Where(x => new[] { "C#", "ASP.NET Core", "EntityFramework", "REST" }.Contains(x.Name))
            .Select(x => new SpecializationTemplateHardSkill
            {
                SpecializationTemplate = specializationOne,
                HardSkillId = x.Id,
                CreatedById = supportUser.UserId,
                CreatedAtUtc = DateTime.UtcNow
            })
            .ToList();

        var specializationTwo = new SpecializationTemplate
        {
            Name = "Azure Cloud Engineer",
            CreatedById = supportUser.UserId,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        specializationTwo.SpecializationTemplateHardSkills = _dbContext.HardSkills
            .Where(x => new[] { "Azure", "Linux", "Azure DevOps Repositories", "Azure DevOps Pipelines", "Azure Service Bus" }.Contains(x.Name))
            .Select(x => new SpecializationTemplateHardSkill
            {
                SpecializationTemplate = specializationTwo,
                HardSkillId = x.Id,
                CreatedById = supportUser.UserId,
                CreatedAtUtc = DateTime.UtcNow
            })
            .ToList();


        _dbContext.SpecializationTemplates.AddRange([
            specializationOne,
            specializationTwo
        ]);
        _dbContext.SaveChanges();

        var user = _dbContext.TeamBoardUsers.First(u => u.Email == "snapshot.test@modsen-software.com");
        var primarySpecialization = new UserSpecialization
        {
            IsPrimary = true,
            IsVisibleSeniorityLevel = true,
            IsVisibleSpecialization = true,
            SpecializationTemplateId = specializationOne.Id,
            UserId = user.UserId,
            CreatedById = user.UserId,
            SeniorityLevelId = "SeniorL2",
            CreatedAtUtc = DateTime.UtcNow
        };
        primarySpecialization.SpecializationHardSkills = specializationOne.SpecializationTemplateHardSkills
            .Select(templateHardSkill => new UserSpecializationHardSkill
            {
                HardSkillId = templateHardSkill.HardSkillId,
                IsVisibleSeniorityLevel = true,
                CreatedById = user.UserId,
                SeniorityLevelId = "Senior",
                CreatedAtUtc = DateTime.UtcNow,
                IsVisibleHardSkill = true,
                UserSpecialization = primarySpecialization
            })
            .ToList();

        var secondarySpecialization = new UserSpecialization
        {
            IsPrimary = true,
            IsVisibleSeniorityLevel = true,
            IsVisibleSpecialization = true,
            SpecializationTemplateId = specializationTwo.Id,
            UserId = user.UserId,
            CreatedById = user.UserId,
            SeniorityLevelId = "JuniorL2",
            CreatedAtUtc = DateTime.UtcNow
        };
        secondarySpecialization.SpecializationHardSkills = specializationTwo.SpecializationTemplateHardSkills
            .Select(templateHardSkill => new UserSpecializationHardSkill
            {
                HardSkillId = templateHardSkill.HardSkillId,
                IsVisibleSeniorityLevel = true,
                CreatedById = user.UserId,
                SeniorityLevelId = "Junior",
                CreatedAtUtc = DateTime.UtcNow,
                IsVisibleHardSkill = true,
                UserSpecialization = secondarySpecialization
            })
            .ToList();


        _dbContext.UserSpecializations.AddRange([
            primarySpecialization,
            secondarySpecialization
        ]);
        _dbContext.SaveChanges();
    }
}
