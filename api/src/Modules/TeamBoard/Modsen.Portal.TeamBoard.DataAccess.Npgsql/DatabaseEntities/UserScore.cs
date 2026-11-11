using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities
{
    internal class UserScore : IAuditableEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TeamBoardUser? User { get; set; }
        public int Score { get; set; }

        public int? CreatedById { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }

    internal class UserScoreConfiguration : IEntityTypeConfiguration<UserScore>
    {
        public void Configure(EntityTypeBuilder<UserScore> builder)
        {
            builder.ToTable("UserScores");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.User)
                .WithOne(x => x.UserScore)
                .HasForeignKey<UserScore>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
