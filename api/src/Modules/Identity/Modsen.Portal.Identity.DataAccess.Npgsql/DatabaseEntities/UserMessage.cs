using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities
{
    internal class UserMessage 
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public int UserId { get; set; }
        public User? CreatedFor { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public MessageRole Role { get; set; }
    }

    internal class UserMessageConfiguration : IEntityTypeConfiguration<UserMessage>
    {
        public void Configure(EntityTypeBuilder<UserMessage> builder)
        {

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired(true);
            builder.Property(x => x.UserId)
                .IsRequired(true);

            builder.Property(x => x.Role)
                .IsRequired(true)
                .HasConversion<string>();

            builder.Property(x => x.Message).HasMaxLength(1024 * 8).IsRequired();

            builder.HasOne(x => x.CreatedFor)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
