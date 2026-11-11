using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class Conversation
{
    public int Id { get; set; }
    public DateOnly? Date { get; set; }
    public ConversationType ConversationType { get; set; }
    public ConversationResult? ConversationResult { get; set; }
    public string? Theses { get; set; }
    public string? EmployeeTheses { get; set; }
    public string? Agreements { get; set; }
    public string? FileId { get; set; }
    public string? FileName { get; set; }

    public int SupervisorId { get; set; }
    public TeamBoardUser? Supervisor { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public List<UserSalary> UserSalaries { get; set; } = [];
    public List<DevelopmentPlan> DevelopmentPlans { get; set; } = [];
    public List<ConversationDocumentLink> ConversationDocumentLinks { get; set; } = [];
    public List<ConversationFile> ConversationFiles { get; set; } = [];
}

internal class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Date).IsRequired(false);

        builder.Property(x => x.ConversationType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.ConversationResult)
            .IsRequired(false);

        builder.Property(x => x.Theses).IsRequired(false);
        builder.Property(x => x.EmployeeTheses).IsRequired(false);
        builder.Property(x => x.Agreements).IsRequired(false);
        builder.Property(x => x.FileId).IsRequired(false);
        builder.Property(x => x.FileName).IsRequired(false);

        builder.HasOne(x => x.Supervisor)
            .WithMany()
            .HasForeignKey(x => x.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.UserSalaries)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.DevelopmentPlans)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ConversationDocumentLinks)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x=> x.ConversationFiles)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
