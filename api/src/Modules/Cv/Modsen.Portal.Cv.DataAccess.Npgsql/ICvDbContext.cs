using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

[assembly: InternalsVisibleTo("Modsen.Portal.IntegrationTests")]
namespace Modsen.Portal.Cv.DataAccess.Npgsql;

internal interface ICvDbContext
{
    public DbSet<CvUserEntity> CvUsers { get; }
    public DbSet<CvClientEntity> CvClients { get; }
    public DbSet<CvProjectEntity> CvProjects { get; }
    public DbSet<CvEntity> Cvs { get; }
    public DbSet<LanguageEntity> Languages { get; }
    public DbSet<ProjectExperienceEntity> ProjectExperiences { get; }
    public DbSet<SkillEntity> Skills { get; }
    public DbSet<SkillGroupEntity> SkillGroups { get; }
    public DbSet<WorkExperienceEntity> WorkExperiences { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
