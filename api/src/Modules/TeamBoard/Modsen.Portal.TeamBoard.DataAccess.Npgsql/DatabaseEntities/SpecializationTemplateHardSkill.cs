using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class SpecializationTemplateHardSkill
{
    public int HardSkillId { get; set; }
    public HardSkill? HardSkill { get; set; }

    public int SpecializationTemplateId { get; set; }
    public SpecializationTemplate? SpecializationTemplate { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

}

internal class SpecializationTemplateHardSkillConfiguration :
    IEntityTypeConfiguration<SpecializationTemplateHardSkill>
{
    public void Configure(EntityTypeBuilder<SpecializationTemplateHardSkill> builder)
    {
        builder.ToTable("SpecializationTemplateHardSkills");

        builder.HasKey(x => new
        {
            x.SpecializationTemplateId,
            x.HardSkillId
        });

        builder.HasOne(x => x.HardSkill)
            .WithMany(x => x.SpecializationTemplateHardSkills)
            .HasForeignKey(x => x.HardSkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SpecializationTemplate)
            .WithMany(x => x.SpecializationTemplateHardSkills)
            .HasForeignKey(x => x.SpecializationTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
