using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

internal class OfficeEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? TimeZone { get; set; }
    public string? TimeZoneDisplayName { get; set; }
    public LocationType LocationType { get; set; }
    public ICollection<Workplace> Workplaces { get; set; } = new List<Workplace>();
    public ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
    public ICollection<MeetingRoomEntity> MeetingRooms { get; set; } = new List<MeetingRoomEntity>();
    public BookingUserEntity? CreatedByUser { get; set; }
    public BookingUserEntity? UpdatedByUser { get; set; }
}

internal class OfficeEntityConfiguration : IEntityTypeConfiguration<OfficeEntity>
{
    public virtual void Configure(EntityTypeBuilder<OfficeEntity> builder)
    {
        builder.ToTable("Offices", BookingDbContext.Schema);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).HasMaxLength(512).IsRequired();

        builder.Property(x => x.TimeZone).HasMaxLength(64);

        builder.Property(x => x.TimeZoneDisplayName).HasMaxLength(64);

        builder.Property(x => x.LocationType).IsRequired();

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
