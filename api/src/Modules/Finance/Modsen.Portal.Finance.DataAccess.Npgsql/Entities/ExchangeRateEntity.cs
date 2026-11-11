using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class ExchangeRateEntity
{
    public int Id { get; set; }
    public required string FromCurrencyId { get; set; }
    public CurrencyEntity? FromCurrency { get; set; }
    public required string ToCurrencyId { get; set; }
    public CurrencyEntity? ToCurrency { get; set; }
    public required int CurrencyMultiplier { get; set; }
    public required decimal Rate { get; set; }
    public required DateOnly ExchangeDate { get; set; }
}

internal class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRateEntity>
{
    public void Configure(EntityTypeBuilder<ExchangeRateEntity> builder)
    {
        builder.ToTable("ExchangeRates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FromCurrencyId).IsRequired().HasMaxLength(3);
        builder.HasOne(x => x.FromCurrency)
            .WithMany()
            .HasForeignKey(x => x.FromCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ToCurrencyId).IsRequired().HasMaxLength(3);
        builder.HasOne(x => x.ToCurrency)
            .WithMany()
            .HasForeignKey(x => x.ToCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CurrencyMultiplier).IsRequired();

        builder.Property(x => x.Rate).IsRequired().HasPrecision(13, 6);

        builder.Property(x => x.ExchangeDate).IsRequired();
    }
}

