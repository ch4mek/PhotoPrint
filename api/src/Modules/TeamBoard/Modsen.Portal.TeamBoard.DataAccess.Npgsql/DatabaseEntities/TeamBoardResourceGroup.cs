using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class TeamBoardResourceGroup
{
    public int ResourceGroupId { get; set; }
    public required string ResourceGroupName { get; set; }
    public bool IsActive { get; set; }
}

internal class TeamBoardResourceGroupConfiguration : IEntityTypeConfiguration<TeamBoardResourceGroup>
{
    public void Configure(EntityTypeBuilder<TeamBoardResourceGroup> builder)
    {
        builder.ToTable("TeamboardResourceGroups");

        builder.HasKey(x => x.ResourceGroupId);
        builder.Property(x => x.ResourceGroupId).ValueGeneratedNever().HasMaxLength(64);

        builder.Property(x => x.ResourceGroupName).HasMaxLength(128).IsRequired();
    }
}
