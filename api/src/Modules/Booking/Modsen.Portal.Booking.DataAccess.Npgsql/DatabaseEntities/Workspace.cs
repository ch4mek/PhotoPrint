using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;

internal class Workspace : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string InternalCode { get; set; }
    public bool IsAvailableForBooking { get; set; }
    public int OfficeId { get; set; }
    public OfficeEntity? Office { get; set; }

    public ICollection<Workplace> Workplaces { get; set; } = new List<Workplace>();
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
    public bool IsClaimRequired { get; set; }
}

internal class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public virtual void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces", BookingDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.InternalCode).HasMaxLength(16).IsRequired();

        builder.Property(x => x.IsAvailableForBooking).HasDefaultValue(true);

        builder.HasIndex(x => new { x.OfficeId, x.Name }).IsUnique();

        builder.HasOne(x => x.Office)
            .WithMany(x => x.Workspaces)
            .HasForeignKey(x => x.OfficeId);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);
        builder.Property(x => x.IsClaimRequired).HasDefaultValue(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById);
    }
}
