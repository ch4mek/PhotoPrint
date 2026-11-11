using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class CurrencyEntity
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string CurrencyCode { get; set; }
    public string? CurrencySymbol { get; set; }
}

internal class CurrencyConfiguration : IEntityTypeConfiguration<CurrencyEntity>
{
    public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
    {
        builder.ToTable("Currencies");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Id).HasMaxLength(3);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);

        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(8);

        builder.Property(x => x.CurrencySymbol).IsRequired(false).HasMaxLength(2);
    }
}
