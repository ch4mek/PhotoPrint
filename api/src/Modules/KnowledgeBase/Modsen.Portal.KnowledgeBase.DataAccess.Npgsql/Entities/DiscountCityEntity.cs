using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class DiscountCityEntity
{
    public int Id { get; set; }

    public int DiscountId { get; set; }
    public DiscountEntity? Discount { get; set; }

    public int CityId { get; set; }
    public CityEntity? City { get; set; }
}

internal class DiscountCityConfiguration : IEntityTypeConfiguration<DiscountCityEntity>
{
    public void Configure(EntityTypeBuilder<DiscountCityEntity> builder)
    {
        builder.ToTable("DiscountCities");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DiscountId).IsRequired();
        builder.Property(x => x.CityId).IsRequired();

        builder.HasOne(x => x.Discount)
            .WithMany(x => x.DiscountCities)
            .HasForeignKey(x => x.DiscountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.City)
            .WithMany(x => x.DiscountCities)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
