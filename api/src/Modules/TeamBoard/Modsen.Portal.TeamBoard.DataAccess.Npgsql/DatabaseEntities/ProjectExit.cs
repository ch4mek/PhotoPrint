using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class ProjectExit : IAuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }
    public int ClientId { get; set; }
    public TeamBoardClient? Client { get; set; }
    public int ProjectId { get; set; }
    public TeamBoardProject? Project { get; set; }
    public DateOnly ClosingDate { get; set; }
    public int ClosingReasonId { get; set; }
    public ProjectExitClosingReason? ClosingReason { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

internal class ProjectFeedbackConfiguration : IEntityTypeConfiguration<ProjectExit>
{
    public void Configure(EntityTypeBuilder<ProjectExit> builder)
    {
        builder.ToTable("ProjectExits");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.UserId).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.ProjectExits)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ProjectId).IsRequired();

        builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectExits)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ClientId).IsRequired();

        builder.HasOne(x => x.Client)
            .WithMany(x => x.ProjectExits)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ClosingDate).IsRequired();

        builder.Property(x => x.ClosingReasonId).IsRequired();

        builder.HasOne(x => x.ClosingReason)
            .WithMany()
            .HasForeignKey(x => x.ClosingReasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.CreatedById);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.Property(x => x.UpdatedById);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UpdatedAtUtc);
    }
}
