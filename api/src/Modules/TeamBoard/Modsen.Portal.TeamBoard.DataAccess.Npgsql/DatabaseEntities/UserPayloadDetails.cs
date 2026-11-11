using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserPayloadDetails : IAuditableEntity
{
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }
    public string? Plan { get; set; }
    public bool NeedProject { get; set; }

    public TeamBoardUser? CreatedByUser { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public TeamBoardUser? UpdatedByUser { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

internal class UserPayloadDetailsConfiguration : IEntityTypeConfiguration<UserPayloadDetails>
{
    public void Configure(EntityTypeBuilder<UserPayloadDetails> builder)
    {
        builder.ToTable("UserPayloadDetails", TeamBoardDbContext.Schema);

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserPayloadDetails)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Plan).IsRequired(false).HasMaxLength(1024);

        builder.Property(x => x.NeedProject).IsRequired().HasDefaultValue(false);

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
    }
}
