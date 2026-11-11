using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class ExternalFeedback : IPortalEntity
{
    public int Id { get; set; }
    public DateOnly ExternalFeedbackDate { get; set; }

    public required string Feedback { get; set; }
    public string? ActionsTodo { get; set; }

    public ExternalFeedbackType ExternalFeedbackType { get; set; }

    public int ProjectId { get; set; }
    public TeamBoardProject? Project { get; set; }

    public int? ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public string? Reviewer { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get ; set ; }
}

internal class ExternalFeedbackConfiguration : IEntityTypeConfiguration<ExternalFeedback>
{
    public void Configure(EntityTypeBuilder<ExternalFeedback> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalFeedbackDate).IsRequired();
        builder.Property(x => x.Feedback).IsRequired();
        builder.Property(x => x.ActionsTodo).IsRequired(false);
        builder.Property(x => x.Reviewer).IsRequired(false);

        builder.Property(x => x.ExternalFeedbackType).IsRequired();
        builder.Property(x => x.ExternalFeedbackType).HasConversion<string>();

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Conversation)
            .WithMany()
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
