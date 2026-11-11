using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

internal class EquipmentUser : AuditableEntity
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public string? FirstNameRu { get; set; }
    public required string LastName { get; set; }
    public string? LastNameRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public string? Position { get; set; }
    public bool IsActive { get; set; }
    public int? ResourceGroupId { get; set; }
    public ICollection<EquipmentAssignEntity> EquipmentAssigns { get; set; } = [];
    public ICollection<EquipmentEntity> ApprovedEquipments { get; set; } = [];
    public ICollection<EquipmentAssignEntity> ApprovedEquipmentAssigns { get; set; } = [];
    public EquipmentUser? CreatedByUser { get; set; }
    public EquipmentUser? UpdatedByUser { get; set; }
}

internal class EquipmentUserConfiguration : IEntityTypeConfiguration<EquipmentUser>
{
    public void Configure(EntityTypeBuilder<EquipmentUser> builder)
    {
        builder.ToTable("EquipmentUsers");

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId)
            .ValueGeneratedNever();

        builder.Property(x => x.DepartmentId)
            .IsRequired(false)
            .HasMaxLength(64);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.FirstNameRu)
            .HasMaxLength(250);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.LastNameRu)
            .HasMaxLength(250);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(PortalSharedConstants.EmailMaxLength);
        builder.HasIndex(x => x.Email);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(value: true);

        builder.Property(x => x.Position)
            .IsRequired(false)
            .HasConversion<string>();

        builder.HasIndex(x => x.IsActive);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
