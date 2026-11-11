using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;

internal class BookingEntry : AuditableEntity
{
    public int Id { get; set; }
    public bool IsBlock { get; set; }
    public int WeeksInInterval { get; set; }
    public bool IsDeleted { get; set; }

    public bool IsForGuest { get; set; }
    public string? GuestName { get; set; }

    public BookingUserEntity? User { get; set; }
    public int UserId { get; set; }

    public Workplace? Workplace { get; set; }
    public int WorkplaceId { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = [];
    public ICollection<BookingNotification> Notifications { get; set; } = [];
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}


internal class BookingEntryConfiguration : IEntityTypeConfiguration<BookingEntry>
{
    public void Configure(EntityTypeBuilder<BookingEntry> builder)
    {
        builder.ToTable("BookingEntries", BookingDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.WorkplaceEntries)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Workplace)
            .WithMany(x => x.BookingEntries)
            .HasForeignKey(x => x.WorkplaceId);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.GuestName).HasMaxLength(512).IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById);

        builder.HasIndex(x => new { x.IsDeleted, x.WorkplaceId, x.GuestName });
    }
}
