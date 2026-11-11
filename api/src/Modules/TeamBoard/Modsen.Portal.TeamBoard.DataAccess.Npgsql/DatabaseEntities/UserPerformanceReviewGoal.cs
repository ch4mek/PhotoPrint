using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal enum UserPerformanceReviewGoalStatus
{
    Pending = 0,
    AcceptedByEmployee = 1,
    Cancelled = 2,
    Completed = 3
}

internal class UserPerformanceReviewGoal
{
    public int Id { get; set; }

    public int UserPerformanceReviewId { get; set; }
    public UserPerformanceReview? UserPerformanceReview { get; set; }

    public required string GoalDescription { get; set; }
    public DateOnly Deadline { get; set; }
    public UserPerformanceReviewGoalStatus Status { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? VerifiedById { get; set; }
    public TeamBoardUser? VerifiedByUser { get; set; }
    public DateTime? VerifiedAtUtc { get; set; }
}

internal class UserPerformanceReviewGoalConfiguration : IEntityTypeConfiguration<UserPerformanceReviewGoal>
{
    public void Configure(EntityTypeBuilder<UserPerformanceReviewGoal> builder)
    {
        builder.ToTable("UserPerformanceReviewGoals");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.UserPerformanceReview)
            .WithMany(x => x.Goals)
            .HasForeignKey(x => x.UserPerformanceReviewId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.GoalDescription).IsRequired().HasMaxLength(1024);

        builder.Property(x => x.Deadline).IsRequired();

        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.VerifiedByUser)
            .WithMany()
            .HasForeignKey(x => x.VerifiedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.VerifiedAtUtc).IsRequired(false);
    }
}
