using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class CityEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<DiscountCityEntity> DiscountCities { get; set; } = [];
}

internal class CityConfiguration : IEntityTypeConfiguration<CityEntity>
{
    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        builder.ToTable("Cities");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
