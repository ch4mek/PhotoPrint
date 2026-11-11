using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class ProjectExperienceEntity : AuditableEntity
{
    public int Id { get; set; }
    public string? Position { get; set; }
    public string? TeamSize { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? Responsibilities { get; set; }
    public string? Environment { get; set; }
    public int OrderWeight { get; set; }
    public int CvId { get; set; }
    public CvEntity? Cv { get; set; }
}

internal class ProjectExperienceConfiguration : IEntityTypeConfiguration<ProjectExperienceEntity>
{
    public void Configure(EntityTypeBuilder<ProjectExperienceEntity> builder)
    {
        builder.ToTable("ProjectExperiences");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Position)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(p => p.TeamSize)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(p => p.StartDate)
            .IsRequired(false);

        builder.Property(p => p.EndDate)
            .IsRequired(false);

        builder.Property(p => p.IsCurrent)
            .IsRequired();

        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(p => p.Responsibilities)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(p => p.Environment)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(p => p.OrderWeight)
            .IsRequired();

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.Property(p => p.UpdatedAtUtc)
            .IsRequired(false);

        builder.HasOne(p => p.Cv)
            .WithMany(c => c.ProjectExperiences)
            .HasForeignKey(p => p.CvId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
