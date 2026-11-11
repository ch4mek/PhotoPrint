using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserSkillsSnapshotSpecialization
{
    public int Id { get; set; }

    public int UserSkillsSnapshotId { get; set; }
    public UserSkillsSnapshot? UserSkillsSnapshot { get; set; }

    public int SpecializationTemplateId { get; set; }
    public SpecializationTemplate? SpecializationTemplate { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }

    public bool UserSpecializationWasPrimary { get; set; }

    public ICollection<UserSkillsSnapshotSpecializationHardSkill> HardSkills { get; set; } =
        new List<UserSkillsSnapshotSpecializationHardSkill>();
}

internal class UserSkillsSnapshotSpecializationConfiguration : IEntityTypeConfiguration<UserSkillsSnapshotSpecialization> {
    public void Configure(EntityTypeBuilder<UserSkillsSnapshotSpecialization> builder)
    {
        builder.ToTable("UserSkillsSnapshotSpecializations", TeamBoardDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.UserSkillsSnapshot)
            .WithMany(x => x.Specializations)
            .HasForeignKey(x => x.UserSkillsSnapshotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SpecializationTemplate)
            .WithMany()
            .HasForeignKey(x => x.SpecializationTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SeniorityLevelId)
            .HasMaxLength(64)
            .IsRequired();
        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.UserSkillsSnapshotId, x.SpecializationTemplateId })
            .IsUnique();
    }
}
