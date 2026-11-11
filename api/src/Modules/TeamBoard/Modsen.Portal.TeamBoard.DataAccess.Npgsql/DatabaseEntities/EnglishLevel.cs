using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class EnglishLevel
{
    public required string Id { get; set; }
    public required string ShortName { get; set; }
    public required string InternalName { get; set; }
    public required string ExternalName { get; set; }
    public required string Description { get; set; }
    public int OrderWeight { get; set; }
}

internal class EnglishLevelConfiguration : IEntityTypeConfiguration<EnglishLevel>
{
    public void Configure(EntityTypeBuilder<EnglishLevel> builder)
    {
        builder.ToTable("EnglishLevels");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Id).HasMaxLength(64);

        builder.Property(x => x.ShortName).HasMaxLength(8).IsRequired();

        builder.Property(x => x.InternalName).HasMaxLength(64).IsRequired();

        builder.Property(x => x.ExternalName).HasMaxLength(64).IsRequired();

        builder.Property(x => x.Description).HasMaxLength(512).IsRequired();

        builder.Property(x => x.OrderWeight).IsRequired();
        builder.HasIndex(x => x.OrderWeight);
    }
}
