using Microsoft.EntityFrameworkCore;

namespace Modsen.Portal.Migrator.DataAccess;

internal class MigratorDbContext : DbContext
{
    public static string Schema => "migration";

    public DbSet<MigrationTask> MigrationTasks { get; set; }

    public MigratorDbContext(DbContextOptions<MigratorDbContext> dbContextOptions)
        : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfiguration(new MigrationTaskConfiguration());
    }
}
