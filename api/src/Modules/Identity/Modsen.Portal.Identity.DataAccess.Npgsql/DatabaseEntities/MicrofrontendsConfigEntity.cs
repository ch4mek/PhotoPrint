using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class MicrofrontendsConfigEntity
{
    public int Id { get; set; }

    public DateTime? LastUpdate { get; set; }

    public string JsonData { get; set; } = string.Empty;
}

internal class MicrofrontendsConfigConfiguration : IEntityTypeConfiguration<MicrofrontendsConfigEntity>
{
    public void Configure(EntityTypeBuilder<MicrofrontendsConfigEntity> builder)
    {
        builder.ToTable("MicrofrontendsConfigs");

        builder.HasKey(config => config.Id);
        builder.Property(config => config.Id).ValueGeneratedOnAdd();

        builder.Property(config => config.LastUpdate)
            .IsRequired(false);

        builder.Property(config => config.JsonData)
            .HasColumnType("json")
            .IsRequired();
    }
}
