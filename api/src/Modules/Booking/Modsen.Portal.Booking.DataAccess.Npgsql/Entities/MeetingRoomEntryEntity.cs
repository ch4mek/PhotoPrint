using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

internal class MeetingRoomEntryEntity : AuditableEntity
{
    public int Id { get; set; }
    public int RepeatEveryWeek { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime ReservationFromUtc { get; set; }
    public DateTime ReservationToUtc { get; set; }
    public DateOnly ReservationPeriodFromOfficeDate { get; set; }
    public DateOnly ReservationPeriodToOfficeDate { get; set; }

    public BookingUserEntity? User { get; set; }
    public int UserId { get; set; }

    public MeetingRoomEntity? MeetingRoom { get; set; }
    public int MeetingRoomId { get; set; }

    public ICollection<MeetingRoomReservationEntity> MeetingRoomReservations { get; set; } = new List<MeetingRoomReservationEntity>();
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class MeetingRoomEntryEntityConfiguration : IEntityTypeConfiguration<MeetingRoomEntryEntity>
{
    public void Configure(EntityTypeBuilder<MeetingRoomEntryEntity> builder)
    {
        builder.ToTable("MeetingRoomEntries", BookingDbContext.Schema);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.MeetingRoom)
            .WithMany(x => x.Entries)
            .HasForeignKey(x => x.MeetingRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.MeetingRoomEntries)
            .HasPrincipalKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.ReservationFromUtc).IsRequired();
        builder.Property(x => x.ReservationToUtc).IsRequired();
        builder.Property(x => x.ReservationPeriodFromOfficeDate).IsRequired();
        builder.Property(x => x.ReservationPeriodToOfficeDate).IsRequired();
        builder.Property(x => x.RepeatEveryWeek).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.HasIndex(x => new { x.IsDeleted, x.RepeatEveryWeek });

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
