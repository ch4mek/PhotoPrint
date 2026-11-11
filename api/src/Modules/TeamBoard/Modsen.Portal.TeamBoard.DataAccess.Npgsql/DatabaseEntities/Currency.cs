using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class Currency
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string CurrencyCode { get; set; }
}

internal class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Id).HasMaxLength(3);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);

        builder.Property(x => x.CurrencyCode).IsRequired(false).HasMaxLength(8);
    }
}
