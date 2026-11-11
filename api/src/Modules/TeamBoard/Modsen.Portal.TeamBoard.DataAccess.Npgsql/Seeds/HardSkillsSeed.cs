using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class HardSkillsSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly TeamBoardDbContext _dbContext;

    public HardSkillsSeed(TeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        foreach (var group in GetHardSkillGroups())
        {
            if (!_dbContext.HardSkillGroups.Any(x => x.Name != group.Name))
            {
                _dbContext.HardSkillGroups.Add(group);
                _dbContext.HardSkills.AddRange(group.HardSkills);
            }
        }

        _dbContext.SaveChanges();
    }

    private static IEnumerable<HardSkillGroup> GetHardSkillGroups()
    {
        yield return Create(group: "Operating Systems", id: "OS", skills: ["Windows", "Linux", "MacOS"]);
        yield return Create(group: "Programming Languages", id: "PL", skills: ["C#", "JavaScript", "Java", "Python", "TypeScript", "C++", "Go", "Rust", "Ruby"]);
        yield return Create(group: "Frameworks", "FW", skills: ["ASP.NET Core", "EntityFramework", "Angular"]);
        yield return Create(group: "API Protocols", "AP", skills: ["REST", "SOAP", "GraphQL", "gRPC"]);
        yield return Create(group: "Libraries", "LIB", skills: ["React"]);
        yield return Create(group: "Cloud Providers", "CP", skills: ["Azure", "AWS", "GCP", "IBM Cloud", "Oracle Cloud"]);
        yield return Create(group: "Databases", "DB", skills: ["Microsoft SQL Server", "PostgreSQL", "MongoDB", "CosmosDB", "Redis"]);
        yield return Create(group: "Integrated Development Environments (IDE)", "IDE", skills: ["Visual Studio", "Visual Studio Code", "JetBrain Rider", "JetBrain WebStorm"]);
        yield return Create(group: "Version Control Systems (VCS)", id: "VCS", skills: ["Git"]);
        yield return Create(group: "Git Environments", "GTE", skills: ["GitLab", "GitHub", "Azure DevOps Repositories", "Bitbucket"]);
        yield return Create(group: "CI/CD", id: "CID", skills: ["GitHub Actions", "GitLab CI/CD", "Azure DevOps Pipelines"]);
        yield return Create(group: "Message Brokers", id: "MB", skills: ["Apache Kafka", "RabbitMQ", "Azure Service Bus"]);
        yield return Create(group: "Tests", id: "TEST", skills: ["xUnit", "MSUnit", "SpecFlow"]);
        yield return Create(group: "Architectures", id: "ARCH", skills: ["Monolith", "Micro-services", "N-Layer", "Onion", "Modular monolith"]);

        static HardSkillGroup Create(string group, string id, params string[] skills)
        {
            return new HardSkillGroup
            {
                Id = id,
                Name = group,
                HardSkills = skills.Select(x => new HardSkill
                {
                    Name = x,
                    CreatedById = null,
                    CreatedAtUtc = DateTime.UtcNow,
                    IsActive = true,
                    HardSkillGroupId = id
                })
                    .ToList()
            };
        }
    }

}
