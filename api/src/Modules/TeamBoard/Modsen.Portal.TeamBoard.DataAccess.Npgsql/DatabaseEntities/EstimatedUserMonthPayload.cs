using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class EstimatedUserMonthPayload : IAuditableEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public int PayloadYear { get; set; }
    public int PayloadMonthOfYear { get; set; }
    public decimal EstimatedFte { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

internal class EstimatedUserMonthPayloadConfiguration : IEntityTypeConfiguration<EstimatedUserMonthPayload>
{
    public void Configure(EntityTypeBuilder<EstimatedUserMonthPayload> builder)
    {
        builder.ToTable("EstimatedUserMonthPayloads", TeamBoardDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.EstimatedUserMonthPayloads)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.PayloadYear).IsRequired();

        builder.Property(x => x.PayloadMonthOfYear).IsRequired();

        builder.Property(x => x.EstimatedFte).HasPrecision(18, 4);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);

        builder.HasIndex(x => new { x.UserId, x.PayloadYear });
    }
}
