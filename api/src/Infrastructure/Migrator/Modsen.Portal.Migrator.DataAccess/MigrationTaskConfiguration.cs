using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Migrator.DataAccess;

internal class MigrationTaskConfiguration : IEntityTypeConfiguration<MigrationTask>
{
    public void Configure(EntityTypeBuilder<MigrationTask> builder)
    {
        builder.ToTable("MigrationTask");

        builder.Property(x => x.TaskName)
            .HasMaxLength(256);

        builder.HasKey(x => x.TaskName);
    }
}
