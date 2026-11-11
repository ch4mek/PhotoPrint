using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class DevelopmentPlan
{
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? DeadlineDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public required string Summary { get; set; }
    public string? Notes { get; set; }
    public DevelopmentPlanSource Source { get; set; }
    public DevelopmentPlanStatusType Status { get; set; }
    public int OrderWeight { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }
    public int? ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
}

internal class DevelopmentPlanConfiguration : IEntityTypeConfiguration<DevelopmentPlan>
{
    public void Configure(EntityTypeBuilder<DevelopmentPlan> builder)
    {
        builder.ToTable("DevelopmentPlans");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.DeadlineDate).IsRequired(false);
        builder.Property(x => x.EndDate).IsRequired(false);
        builder.Property(x => x.Summary).IsRequired();
        builder.Property(x => x.Notes).IsRequired(false);
        builder.Property(x => x.Source).IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(DevelopmentPlanSource.Unknown);

        builder.Property(x => x.Status).IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(DevelopmentPlanStatusType.None);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.DevelopmentPlans)
            .HasForeignKey(x => x.ConversationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
