using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class PortalSetting
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Value { get; set; }
    public AllowedValueType? ValueType { get; set; }
}

internal class PortalSettingConfiguration : IEntityTypeConfiguration<PortalSetting>
{
    public void Configure(EntityTypeBuilder<PortalSetting> builder)
    {
        builder.ToTable("PortalSettings", IdentityModuleDbContext.Schema);

        builder.HasKey(portalSetting => portalSetting.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(portalSetting => portalSetting.Name)
            .IsRequired()
            .HasMaxLength(128);
        builder.HasIndex(portalSetting => portalSetting.Name)
            .IsUnique();

        builder.Property(portalSetting => portalSetting.Value)
            .IsRequired(false)
            .HasMaxLength(128);

        builder.Property(portalSetting => portalSetting.ValueType)
            .IsRequired(false)
            .HasConversion(new EnumConverter<AllowedValueType>());
    }
}
