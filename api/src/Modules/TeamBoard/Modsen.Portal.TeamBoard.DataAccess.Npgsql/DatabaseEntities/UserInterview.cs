using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserInterview : IAuditableEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public required string Specialization { get; set; }

    public DateOnly? UserInterviewDate { get; set; }
    public TimeOnly? UserInterviewFromUtc { get; set; }
    public TimeOnly? UserInterviewToUtc { get; set; }

    public string? ClientName { get; set; }
    public UserInterviewStatus? Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public UserInterviewType? InterviewType { get; set; }
    public int InterviewTypeOrderWeight { get; set; }

    public int? SalesResponsibleUserId { get; set; }
    public TeamBoardUser? SalesResponsibleUser { get; set; }

    public string? JiraTask { get; set; }
    public string? Notes { get; set; }
    public string? ReasonForRefusal { get; set; }

    public Guid GroupId { get; set; }

    // not used for now
    public UserInterviewLanguage? Language { get; set; }
    public string? RequiredLocation { get; set; }
    public string? RequiredEnglishLevelId { get; set; }
    public EnglishLevel? RequiredEnglishLevel { get; set; }

    public bool IsActive { get; set; }
    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

internal class UserInterviewConfiguration : IEntityTypeConfiguration<UserInterview>
{
    public void Configure(EntityTypeBuilder<UserInterview> builder)
    {
        builder.ToTable("UserInterviews");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Specialization).IsRequired().HasMaxLength(250);
        builder.Property(x => x.UserInterviewDate).IsRequired(false);
        builder.Property(x => x.UserInterviewFromUtc).IsRequired(false);
        builder.Property(x => x.UserInterviewToUtc).IsRequired(false);
        builder.Property(x => x.ClientName).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.JiraTask).IsRequired(false).HasMaxLength(1000);
        builder.Property(x => x.Status).IsRequired(false).HasConversion<string>();
        builder.Property(x => x.StatusOrderWeight).HasComputedColumnSql(
            """CASE "Status" WHEN 'WaitingForInterview' THEN 100 WHEN 'WaitingForResult' THEN 200 WHEN 'Failed' THEN 300 WHEN 'Success' THEN 400 WHEN 'Started' THEN 500 WHEN 'NotStarted' THEN 600 WHEN 'Canceled' THEN 700 WHEN 'NoFeedback' THEN 800 WHEN 'NoNextStepScheduled' THEN 900 ELSE 1000 END""",
            stored: true);
        builder.Property(x => x.InterviewType).IsRequired(false).HasConversion<string>();
        builder.Property(x => x.InterviewTypeOrderWeight).HasComputedColumnSql(
            """CASE "InterviewType" WHEN 'Prescreen' THEN 100 WHEN 'Tech' THEN 200 WHEN 'Final' THEN 300 WHEN 'Other' THEN 400 ELSE 500 END""",
            stored: true
            );
        builder.Property(x => x.ReasonForRefusal).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.Notes).IsRequired(false).HasMaxLength(1000);
        builder.Property(x => x.GroupId).IsRequired();
        builder.HasIndex(x => x.GroupId);
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserInterviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.SalesResponsibleUser)
            .WithMany(x => x.ManagedUserInterviews)
            .HasForeignKey(x => x.SalesResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);

        // not used for now
        builder.Property(x => x.Language).IsRequired(false).HasConversion<string>();
        builder.Property(x => x.RequiredLocation).IsRequired(false).HasMaxLength(128);
        builder.HasOne(x => x.RequiredEnglishLevel)
            .WithMany()
            .HasForeignKey(x => x.RequiredEnglishLevelId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
