using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class ProjectUser
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public int ProjectId { get; set; }
    public TeamBoardProject? Project { get; set; }

    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsTimeOnProjectBillable { get; set; }

    public string? UserOnProjectSeniorityLevelId { get; set; }
    public SeniorityLevel? UserOnProjectSeniorityLevel { get; set; }

    public decimal Fte { get; set; }

    public int? RefUserId { get; set; }
    public TeamBoardUser? RefUser { get; set; }

    public string? Comments { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

internal class ProjectUserConfiguration : IEntityTypeConfiguration<ProjectUser>
{
    public void Configure(EntityTypeBuilder<ProjectUser> builder)
    {
        builder.ToTable("ProjectUsers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserProjects)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UserOnProjectFrom).IsRequired();

        builder.Property(x => x.UserOnProjectTo).IsRequired(false);

        builder
            .HasIndex(x => new { x.UserOnProjectFrom, x.UserOnProjectTo, x.UserId });

        builder.HasOne(x => x.UserOnProjectSeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.UserOnProjectSeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Fte).HasPrecision(18, 4);

        builder.HasOne(x => x.RefUser)
            .WithMany()
            .HasForeignKey(x => x.RefUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Comments).IsRequired(false).HasMaxLength(512);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        
        builder.Property(x => x.IsTimeOnProjectBillable).IsRequired();
    }
}
