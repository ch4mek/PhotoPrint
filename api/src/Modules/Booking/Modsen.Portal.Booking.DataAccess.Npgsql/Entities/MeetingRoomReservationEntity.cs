using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

internal class MeetingRoomReservationEntity : AuditableEntity
{
    public int Id { get; set; }

    public int MeetingRoomEntryId { get; set; }
    public MeetingRoomEntryEntity? MeetingRoomEntry { get; set; }

    public int UserId { get; set; }
    public BookingUserEntity? User { get; set; }

    public DateTime ReservationFromUtc { get; set; }
    public DateTime ReservationToUtc { get; set; }

    public bool IsDeleted { get; set; }
    public string? Comments { get; set; }

    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class MeetingRoomReservationEntityConfiguration : IEntityTypeConfiguration<MeetingRoomReservationEntity>
{
    public virtual void Configure(EntityTypeBuilder<MeetingRoomReservationEntity> builder)
    {
        builder.ToTable("MeetingRoomReservations", BookingDbContext.Schema);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.MeetingRoomEntry)
            .WithMany(x => x.MeetingRoomReservations)
            .HasForeignKey(x => x.MeetingRoomEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ReservationFromUtc).IsRequired();
        builder.HasIndex(x => x.ReservationFromUtc);

        builder.Property(x => x.ReservationToUtc).IsRequired();
        builder.HasIndex(x => x.ReservationToUtc);

        builder.Property(x => x.IsDeleted).IsRequired();
        builder.HasIndex(x => x.IsDeleted);

        builder.Property(x => x.Comments)
            .IsRequired(false)
            .HasMaxLength(512);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

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
