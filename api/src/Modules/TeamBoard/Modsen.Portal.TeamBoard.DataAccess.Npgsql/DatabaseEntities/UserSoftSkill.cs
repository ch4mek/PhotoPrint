using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserSoftSkill
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public int SoftSkillId { get; set; }
    public SoftSkill? SoftSkill { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }

    public bool IsVisibleSoftSkill { get; set; }
    public bool IsVisibleSeniorityLevel { get; set; }

    public int? UserSoftSkillsSnapshotId { get; set; }
    public UserSoftSkillsSnapshot? UserSoftSkillsSnapshot { get; set; }
}

internal class UserSoftSkillConfiguration : IEntityTypeConfiguration<UserSoftSkill>
{
    public void Configure(EntityTypeBuilder<UserSoftSkill> builder)
    {
        builder.ToTable("UserSoftSkills");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserSoftSkills)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SoftSkill)
            .WithMany()
            .HasForeignKey(x => x.SoftSkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsVisibleSeniorityLevel).IsRequired();

        builder.Property(x => x.IsVisibleSoftSkill).IsRequired();
    }
}
