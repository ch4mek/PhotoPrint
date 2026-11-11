using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql;

internal class CvDbContext : DbContext, ICvDbContext
{
    public const string Schema = "cv";

    public DbSet<CvUserEntity> CvUsers { get; set; }
    public DbSet<CvClientEntity> CvClients { get; set; }
    public DbSet<CvProjectEntity> CvProjects { get; set; }
    public DbSet<CvEntity> Cvs { get; set; }
    public DbSet<LanguageEntity> Languages { get; set; }
    public DbSet<ProjectExperienceEntity> ProjectExperiences { get; set; }
    public DbSet<SkillEntity> Skills { get; set; }
    public DbSet<SkillGroupEntity> SkillGroups { get; set; }
    public DbSet<WorkExperienceEntity> WorkExperiences { get; set; }

    public CvDbContext(DbContextOptions<CvDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(Schema)
            .ApplyConfigurationsFromAssembly(typeof(CvDbContext).Assembly);
    }
}
