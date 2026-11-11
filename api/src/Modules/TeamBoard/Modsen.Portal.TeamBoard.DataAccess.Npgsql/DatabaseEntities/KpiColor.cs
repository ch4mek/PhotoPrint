using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class KpiColor
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int RightIntervalBorder { get; set; }
    public int ColorOrderWeight { get; set; }
}

internal class KpiColorConfiguration : IEntityTypeConfiguration<KpiColor>
{
    public void Configure(EntityTypeBuilder<KpiColor> builder)
    {
        builder.ToTable("KpiColors", table =>
            table.HasCheckConstraint("CK_KpiColors_RightIntervalBorder",
                "\"RightIntervalBorder\" <= 9999 AND \"RightIntervalBorder\" > 0"));

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.RightIntervalBorder).IsRequired();

        builder.Property(x => x.ColorOrderWeight).IsRequired();

        builder.Property(x => x.Name).IsRequired();
    }
}
