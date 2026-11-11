using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class InternshipSpecializationHardSkill : IAuditableEntity
{
    public int InternsipSpecializationId { get; set; }
    public InternshipSpecialization? InternshipSpecialization { get; set; }

    public int HardSkillId { get; set; }
    public HardSkill? HardSkill { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
}

internal class InternshipSpecializationHardSkillConfiguration : IEntityTypeConfiguration<InternshipSpecializationHardSkill>
{
    public void Configure(EntityTypeBuilder<InternshipSpecializationHardSkill> builder)
    {
        builder.ToTable("InternshipSpecializationHardSkills");

        builder.HasKey(x => new
        {
            x.InternsipSpecializationId,
            x.HardSkillId
        });

        builder.HasOne(x => x.InternshipSpecialization)
            .WithMany(x => x.InternshipSpecializationHardSkills)
            .HasForeignKey(x => x.InternsipSpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HardSkill)
            .WithMany()
            .HasForeignKey(x => x.HardSkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

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
