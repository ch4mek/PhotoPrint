using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

internal class BookingUserEntity : AuditableEntity
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public bool IsActive { get; set; }
    public ICollection<BookingEntry> WorkplaceEntries { get; set; } = new List<BookingEntry>();
    public ICollection<MeetingRoomEntryEntity> MeetingRoomEntries { get; set; } = new List<MeetingRoomEntryEntity>();
    public BookingUserEntity? UpdatedByUser { get; set; }
  }

internal class BookingUserEntityConfiguration : IEntityTypeConfiguration<BookingUserEntity>
{
    public void Configure(EntityTypeBuilder<BookingUserEntity> builder)
    {
        builder.ToTable("BookingUsers", BookingDbContext.Schema);

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property(x => x.DepartmentId).HasMaxLength(64).IsRequired(false);

        builder.Property(x => x.FirstName).HasMaxLength(250).IsRequired();

        builder.Property(x => x.LastName).HasMaxLength(250).IsRequired();

        builder.Property(x => x.FirstNameRu).HasMaxLength(250).IsRequired(false);

        builder.Property(x => x.LastNameRu).HasMaxLength(250).IsRequired(false);

        builder.Property(x => x.Email).IsRequired()
            .HasMaxLength(PortalSharedConstants.EmailMaxLength);
        builder.HasIndex(x => x.Email);

        builder.Property(x => x.ResourceGroupId).IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(value: true);
        builder.HasIndex(x => x.IsActive);

        builder.Property(x => x.CreatedById).IsRequired(false);
        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
