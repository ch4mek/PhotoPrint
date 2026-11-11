using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserSkillsSnapshotSpecializationHardSkill
{
    public int UserSkillsSnapshotSpecializationId { get; set; }
    public UserSkillsSnapshotSpecialization? UserSkillsSnapshotSpecialization { get; set; }

    public int HardSkillId { get; set; }
    public HardSkill? HardSkill { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }
}

internal class UserSkillsSnapshotSpecializationHardSkillConfiguration :
    IEntityTypeConfiguration<UserSkillsSnapshotSpecializationHardSkill>
{
    public void Configure(EntityTypeBuilder<UserSkillsSnapshotSpecializationHardSkill> builder)
    {
        builder.ToTable("UserSkillsSnapshotSpecializationHardSkills", TeamBoardDbContext.Schema);

        builder.HasKey(x => new { x.UserSkillsSnapshotSpecializationId, x.HardSkillId });

        builder.HasOne(x => x.UserSkillsSnapshotSpecialization)
            .WithMany(x => x.HardSkills)
            .HasForeignKey(x => x.UserSkillsSnapshotSpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HardSkill)
            .WithMany()
            .HasForeignKey(x => x.HardSkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SeniorityLevelId)
            .HasMaxLength(64)
            .IsRequired(false);
        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
