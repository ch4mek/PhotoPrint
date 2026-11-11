using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class Country
{
   public int Id { get; set; }
   public required string Name { get; set; }
}

internal class CountryConfiguration : IEntityTypeConfiguration<Country>
{
   public void Configure(EntityTypeBuilder<Country> builder)
   {
       builder.ToTable("Countries");

       builder.HasKey(x => x.Id);
       builder.Property(x => x.Id).ValueGeneratedOnAdd();

       builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
   }
}
