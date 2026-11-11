using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Notification;

namespace Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

internal class NotificationEntity
{
    public int Id { get; set; }
    public required string Arguments { get; set; }
    public int UserId { get; set; }

    public NotificationChannels NotificationChannel { get; set; }

    public bool Sent { get; set; }
    public bool Read { get; set; }
    public bool IsDeleted { get; set; }

    public NotificationTypes NotificationType { get; set; }
}

internal class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("Notifications", NotificationDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Arguments)
               .IsRequired()
               .HasMaxLength(2048)
               .HasColumnType("json");

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.NotificationType)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(x => x.NotificationChannel)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(x => x.Sent)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.Read)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);

        builder.HasIndex(x => new { x.NotificationChannel, x.Sent, x.IsDeleted });
    }
}
