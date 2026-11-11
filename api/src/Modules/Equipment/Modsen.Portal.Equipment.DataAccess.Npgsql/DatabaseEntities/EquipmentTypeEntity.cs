using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

internal class EquipmentTypeEntity : AuditableEntity
{
    public required string Id { get; set; }
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
    public required int OrderWeight { get; set; }
    public EquipmentUser? CreatedByUser { get; set; }
    public EquipmentUser? UpdatedByUser { get; set; }
}

internal class EquipmentTypeConfiguration : IEntityTypeConfiguration<EquipmentTypeEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentTypeEntity> builder)
    {
        builder.ToTable("EquipmentTypes", EquipmentDbContext.Schema);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
