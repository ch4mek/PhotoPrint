using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;

namespace Modsen.Portal.Notification.DataAccess.Npgsql.Entities
{
    internal class NotificationUserEntity
    {
        public int UserId { get; set; }

        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? AvatarFileId { get; set; }
        public bool IsActive { get; set; }
    }

    internal class NotificationUserConfiguration : IEntityTypeConfiguration<NotificationUserEntity>
    {
        public void Configure(EntityTypeBuilder<NotificationUserEntity> builder)
        {
            builder.ToTable("NotificationUsers", NotificationDbContext.Schema);

            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserId)
                .ValueGeneratedNever();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(PortalSharedConstants.EmailMaxLength);
            builder.HasIndex(x => x.Email);

            builder.Property(x => x.FirstName).HasMaxLength(250);
            builder.Property(x => x.LastName).HasMaxLength(250);

            builder.Property(x => x.AvatarFileId).IsRequired(false).HasMaxLength(128);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
