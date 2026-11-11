using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;

internal class BookingNotification : AuditableEntity
{
    public int Id { get; set; }

    public BookingEntry? BookingEntry { get; set; }
    public int BookingEntryId { get; set; }

    public string? Message { get; set; }
    public bool WasRead { get; set; }

    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class BookingNotificationConfiguration : IEntityTypeConfiguration<BookingNotification>
{
    public void Configure(EntityTypeBuilder<BookingNotification> builder)
    {
        builder.ToTable("BookingNotifications", BookingDbContext.Schema);
       
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Message)
            .HasMaxLength(4096)
            .IsRequired(false);

        builder.Property(x => x.WasRead).IsRequired();
        builder.HasIndex(x => x.WasRead);

        builder.HasOne(x => x.BookingEntry)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.BookingEntryId);

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
