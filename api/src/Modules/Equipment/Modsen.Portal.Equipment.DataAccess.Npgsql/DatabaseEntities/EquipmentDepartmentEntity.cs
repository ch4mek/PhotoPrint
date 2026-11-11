using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

internal class EquipmentDepartmentEntity
{
    public required string DepartmentId { get; set; }
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
}

internal class EquipmentDepartmentConfiguration : IEntityTypeConfiguration<EquipmentDepartmentEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentDepartmentEntity> builder)
    {
        builder.ToTable("EquipmentDepartments");

        builder.HasKey(x => x.DepartmentId);
        builder.Property(x => x.DepartmentId).ValueGeneratedNever().HasMaxLength(64);

        builder.Property(x => x.ShortName).IsRequired().HasMaxLength(32);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(128);
    }
}
