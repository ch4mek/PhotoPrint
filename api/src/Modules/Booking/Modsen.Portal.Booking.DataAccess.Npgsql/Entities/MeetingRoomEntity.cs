using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

internal class MeetingRoomEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CountOfSeats { get; set; }
    public string? Comments { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    public int OfficeId { get; set; }
    public OfficeEntity? Office { get; set; }

    public ICollection<MeetingRoomEntryEntity> Entries { get; set; } = new List<MeetingRoomEntryEntity>();
    public ICollection<MeetingRoomItemEntity> Items { get; set; } = new List<MeetingRoomItemEntity>();
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class MeetingRoomEntityConfiguration : IEntityTypeConfiguration<MeetingRoomEntity>
{
    public virtual void Configure(EntityTypeBuilder<MeetingRoomEntity> builder)
    {
        builder.ToTable("MeetingRooms", BookingDbContext.Schema);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => new { x.OfficeId, x.Name }).IsUnique();

        builder.HasOne(x => x.Office)
            .WithMany(x => x.MeetingRooms)
            .HasForeignKey(x => x.OfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Comments)
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(x => x.CountOfSeats)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

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
