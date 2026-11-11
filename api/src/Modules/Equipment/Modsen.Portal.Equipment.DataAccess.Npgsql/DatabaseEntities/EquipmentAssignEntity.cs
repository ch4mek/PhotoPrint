using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

internal class EquipmentAssignEntity : AuditableEntity
{
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public int AssignedToUserId { get; set; }
    public EquipmentUser? User { get; set; }

    public int? ApproverId { get; set; }
    public EquipmentUser? Approver { get; set; }

    public int EquipmentId { get; set; }
    public EquipmentEntity? Equipment { get; set; }

    public bool IsDeleted { get; set; }
    public EquipmentUser? CreatedByUser { get; set; }
    public EquipmentUser? UpdatedByUser { get; set; }
}

internal class EquipmentAssignConfiguration : IEntityTypeConfiguration<EquipmentAssignEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentAssignEntity> builder)
    {
        builder.ToTable("EquipmentAssigns");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
        builder.Property(x => x.CreatedById)
            .IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired(false);
        builder.Property(x => x.UpdatedById)
            .IsRequired(false);

        builder.Property(x => x.IssueDate)
            .IsRequired();

        builder.Property(x => x.ReturnDate)
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.EquipmentAssigns)
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Approver)
            .WithMany(x => x.ApprovedEquipmentAssigns)
            .HasForeignKey(x => x.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Equipment)
            .WithMany(x => x.Assigns)
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById);
    }
}
