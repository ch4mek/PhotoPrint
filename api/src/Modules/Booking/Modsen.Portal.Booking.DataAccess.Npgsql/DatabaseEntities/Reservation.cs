using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;

internal class Reservation : AuditableEntity
{
    public int Id { get; set; }
    public DateTime ReservationDate { get; set; }
    public int ReservationYear { get; set; }
    public int ReservationForMonth { get; set; }
    public int ReservationForWeek { get; set; }
    public int ReservationForDay { get; set; }
    public bool IsDeleted { get; set; }

    public BookingEntry? BookingEntry { get; set; }
    public int BookingEntryId { get; set; }
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public virtual void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations", BookingDbContext.Schema);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ReservationDate).IsRequired();
        builder.Property(x => x.ReservationYear).IsRequired();
        builder.Property(x => x.ReservationForMonth).IsRequired();
        builder.Property(x => x.ReservationForWeek).IsRequired();
        builder.Property(x => x.ReservationForDay).IsRequired();

        builder.HasOne(x => x.BookingEntry)
            .WithMany(x => x.Reservations)
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
