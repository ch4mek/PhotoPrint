using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class ProjectExitClosingReason
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

internal class ProjectExitClosingReasonConfiguration : IEntityTypeConfiguration<ProjectExitClosingReason>
{
    public void Configure(EntityTypeBuilder<ProjectExitClosingReason> builder)
    {
        builder.ToTable("ProjectExitClosingReasons");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(512);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
