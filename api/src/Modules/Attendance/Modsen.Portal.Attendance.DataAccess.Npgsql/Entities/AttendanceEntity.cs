using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;

internal class AttendanceEntity
{
    public int Id { get; set; }
    public DateTime CheckedAtUtc { get; set; }
    public LocationType Location { get; set; }
    public DirectionType Direction { get; set; }
    public required string Door { get; set; }

    public int UserId { get; set; }
    public UserEntity? User { get; set; }
}

internal class AttendanceEntityConfiguration : IEntityTypeConfiguration<AttendanceEntity>
{
    public void Configure(EntityTypeBuilder<AttendanceEntity> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CheckedAtUtc).IsRequired();

        builder.Property(x => x.Location)
            .IsRequired()
            .HasDefaultValue(LocationType.Unknown);

        builder.Property(x => x.Door)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Direction).IsRequired();

        builder.HasOne(x => x.User).WithMany(x => x.Attendances).HasForeignKey(x => x.UserId);
    }
}
