using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Booking;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

internal class MeetingRoomItemEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public MeetingRoomItemType ItemType { get; set; }
    public int Count { get; set; }
    public int MeetingRoomId { get; set; }
    public MeetingRoomEntity? MeetingRoom { get; set; }
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class MeetingRoomItemEntityConfiguration : IEntityTypeConfiguration<MeetingRoomItemEntity>
{
    public void Configure(EntityTypeBuilder<MeetingRoomItemEntity> builder)
    {
        builder.ToTable("MeetingRoomItems", BookingDbContext.Schema);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.MeetingRoom)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.MeetingRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ItemType)
            .IsRequired()
            .HasConversion<string>();

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
