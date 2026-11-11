using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class CvProjectEntity : AuditableEntity
{
    public required string ProjectId { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public ICollection<CvEntity> Cvs { get; set; } = [];
}

internal class CvProjectConfiguration : IEntityTypeConfiguration<CvProjectEntity>
{
    public void Configure(EntityTypeBuilder<CvProjectEntity> builder)
    {
        builder.ToTable("CvProjects");

        builder.HasKey(p => p.ProjectId);
        builder.Property(p => p.ProjectId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.Property(p => p.UpdatedAtUtc)
            .IsRequired(false);
    }
}
