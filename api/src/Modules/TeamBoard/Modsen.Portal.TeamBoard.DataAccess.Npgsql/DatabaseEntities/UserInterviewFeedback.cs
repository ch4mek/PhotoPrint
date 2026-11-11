using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal enum UserInterviewFeedbackType
{
    FromEmployee = 0,
    FromClient = 1,
    FromResponsibleUser = 2
}

internal class UserInterviewFeedback
{
    public int Id { get; set; }

    public int UserInterviewId { get; set; }
    public UserInterview? UserInterview { get; set; }

    public DateOnly FeedbackDate { get; set; }
    public UserInterviewFeedbackType Type { get; set; }
    public required string Feedback { get; set; }
    public string ActionsTodo { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

internal class UserInterviewFeedbackConfiguration : IEntityTypeConfiguration<UserInterviewFeedback>
{
    public void Configure(EntityTypeBuilder<UserInterviewFeedback> builder)
    {
        builder.ToTable("UserInterviewFeedbacks");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FeedbackDate).IsRequired();

        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>();

        builder.Property(x => x.Feedback).IsRequired().HasMaxLength(1024);

        builder.Property(x => x.Feedback).IsRequired(false).HasMaxLength(1024);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
    }
}
