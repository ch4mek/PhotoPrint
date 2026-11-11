using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;


internal class UserPerformanceReview
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public DateTime? InterviewDateFromUtc { get; set; }
    public DateTime? InterviewDateToUtc { get; set; }
    public UserPerformanceReviewType Type { get; set; }
    public UserPerformanceReviewStatus Status { get; set; }
    public string? EmployeeFeedback { get; set; }
    public string? Agreements { get; set; }

    public int? ResponsibleUserId { get; set; }
    public TeamBoardUser? ResponsibleUser { get; set; }
    public string? ResponsibleUserFeedback { get; set; }

    public int? ChiefLevelManagerId { get; set; }
    public TeamBoardUser? ChiefLevelManager { get; set; }
    public string? ChiefLevelManagerFeedback { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public int? ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public List<UserPerformanceReviewGoal> Goals { get; set; } = [];
}

internal class UserPerformanceReviewConfiguration : IEntityTypeConfiguration<UserPerformanceReview>
{
    public void Configure(EntityTypeBuilder<UserPerformanceReview> builder)
    {
        builder.ToTable("UserPerformanceReviews", table =>
        {
            table.HasCheckConstraint("CK_UserPerformanceReview_InterviewDateTimeRange", "`InterviewDateToUtc` > `InterviewDateFromUtc`");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserPerformanceReviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.InterviewDateFromUtc).IsRequired();

        builder.Property(x => x.InterviewDateToUtc).IsRequired();

        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>();

        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>();

        builder.Property(x => x.EmployeeFeedback).IsRequired(false).HasMaxLength(5000);

        builder.Property(x => x.Agreements).IsRequired(false).HasMaxLength(5000);

        builder.HasOne(x => x.ResponsibleUser)
            .WithMany()
            .HasForeignKey(x => x.ResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ResponsibleUserFeedback).IsRequired(false).HasMaxLength(5000);

        builder.HasOne(x => x.ChiefLevelManager)
            .WithMany()
            .HasForeignKey(x => x.ChiefLevelManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ChiefLevelManagerFeedback).IsRequired(false).HasMaxLength(1024);

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

        builder.HasOne(x => x.Conversation)
            .WithMany()
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
