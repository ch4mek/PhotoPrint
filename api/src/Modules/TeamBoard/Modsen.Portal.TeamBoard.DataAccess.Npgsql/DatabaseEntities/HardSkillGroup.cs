using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class HardSkillGroup
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public List<HardSkill> HardSkills { get; set; } = [];
}

internal class HardSkillGroupConfiguration : IEntityTypeConfiguration<HardSkillGroup>
{
    public void Configure(EntityTypeBuilder<HardSkillGroup> builder)
    {
        builder.ToTable("HardSkillGroups");

        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Id).HasMaxLength(8);

        builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
    }
}
