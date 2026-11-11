using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserSpecializationHardSkill : IAuditableEntity
{
    public int UserSpecializationId { get; set; }
    public UserSpecialization? UserSpecialization { get; set; }

    public int HardSkillId { get; set; }
    public HardSkill? HardSkill { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }

    public bool IsVisibleHardSkill { get; set; }
    public bool IsVisibleSeniorityLevel { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
}

internal class UserSpecializationHardSkillConfiguration : IEntityTypeConfiguration<UserSpecializationHardSkill>
{
    public void Configure(EntityTypeBuilder<UserSpecializationHardSkill> builder)
    {
        builder.ToTable("UserSpecializationHardSkills");

        builder.HasKey(x => new
        {
            x.UserSpecializationId,
            x.HardSkillId
        });

        builder.HasOne(x => x.UserSpecialization)
            .WithMany(x => x.SpecializationHardSkills)
            .HasForeignKey(x => x.UserSpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HardSkill)
            .WithMany()
            .HasForeignKey(x => x.HardSkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsVisibleSeniorityLevel).IsRequired();

        builder.Property(x => x.IsVisibleHardSkill).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
