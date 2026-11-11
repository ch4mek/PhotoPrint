using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class HardSkill
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public required string HardSkillGroupId { get; set; }
    public HardSkillGroup? Group { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<SpecializationTemplateHardSkill> SpecializationTemplateHardSkills { get; set; } = [];
}

internal class HardSkillConfiguration : IEntityTypeConfiguration<HardSkill>
{
    public void Configure(EntityTypeBuilder<HardSkill> builder)
    {
        builder.ToTable("HardSkills");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.HardSkillGroupId).IsRequired().HasMaxLength(8);
        builder.HasOne(x => x.Group)
            .WithMany(x => x.HardSkills)
            .HasForeignKey(x => x.HardSkillGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
    }
}
