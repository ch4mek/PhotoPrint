using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class WorkExperienceEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Specialization { get; set; }
    public int ExperienceInYears { get; set; }
    public int CvId { get; set; }
    public CvEntity? Cv { get; set; }
}

internal class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperienceEntity>
{
    public void Configure(EntityTypeBuilder<WorkExperienceEntity> builder)
    {
        builder.ToTable("WorkExperiences");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .ValueGeneratedOnAdd();

        builder.Property(w => w.Specialization)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(w => w.ExperienceInYears)
            .IsRequired();

        builder.Property(w => w.CreatedAtUtc)
            .IsRequired();

        builder.Property(w => w.UpdatedAtUtc)
            .IsRequired(false);

        builder.HasOne(w => w.Cv)
            .WithMany(c => c.WorkExperiences)
            .HasForeignKey(w => w.CvId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
