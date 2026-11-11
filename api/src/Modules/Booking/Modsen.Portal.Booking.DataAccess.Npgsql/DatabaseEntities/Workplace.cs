using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;

internal class Workplace : AuditableEntity
{
    public int Id { get; set; }
    public int WorkplaceNumber { get; set; }

    public OfficeEntity? Office { get; set; }
    public int OfficeId { get; set; }

    public int? WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    public ICollection<BookingEntry> BookingEntries { get; set; } = new List<BookingEntry>();
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class WorkplaceConfiguration : IEntityTypeConfiguration<Workplace>
{
    public virtual void Configure(EntityTypeBuilder<Workplace> builder)
    {
        builder.ToTable("Workplaces", BookingDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.WorkplaceNumber).IsRequired();

        builder.HasIndex(x => new { x.OfficeId, x.WorkspaceId, x.WorkplaceNumber }).IsUnique();

        builder.HasOne(x => x.Office)
            .WithMany(x => x.Workplaces)
            .HasForeignKey(x => x.OfficeId);

        builder.Property(x => x.WorkspaceId).IsRequired(false);
        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.Workplaces)
            .HasForeignKey(x => x.WorkspaceId);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById);
    }
}
