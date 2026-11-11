using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class SkillEntity : AuditableEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public CvSkillLevel? Level { get; set; }
    public int? ExperienceInYears { get; set; }
    public int? LastUsedYear { get; set; }
    public int OrderWeight { get; set; }
    public int SkillGroupId { get; set; }
    public SkillGroupEntity? SkillGroup { get; set; }
}

internal class SkillConfiguration : IEntityTypeConfiguration<SkillEntity>
{
    public void Configure(EntityTypeBuilder<SkillEntity> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(s => s.Level)
            .IsRequired(false)
            .HasConversion<string>();

        builder.Property(s => s.ExperienceInYears)
            .IsRequired(false);

        builder.Property(s => s.LastUsedYear)
            .IsRequired(false);

        builder.Property(s => s.OrderWeight)
            .IsRequired();

        builder.Property(s => s.CreatedAtUtc)
            .IsRequired();

        builder.Property(s => s.UpdatedAtUtc)
            .IsRequired(false);

        builder.HasOne(s => s.SkillGroup)
            .WithMany(g => g.Skills)
            .HasForeignKey(s => s.SkillGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
