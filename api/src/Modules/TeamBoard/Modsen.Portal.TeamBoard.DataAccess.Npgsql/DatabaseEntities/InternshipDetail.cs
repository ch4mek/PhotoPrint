using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class InternshipDetail : IAuditableEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public required string Specialization { get; set; }
    public int? ApprovedPoints { get; set; }
    public int? CompletedPoints { get; set; }
    public int EngineerStage { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly PlannedEndDate { get; set; }

    public InternshipType Type { get; set; }
    public int TypeOrderWeight { get; set; }

    public InternshipFinalResult? FinalResult { get; set; }
    public int FinalResultOrderWeight { get; set; }

    public TeamBoardUser? CreatedByUser { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public TeamBoardUser? UpdatedByUser { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public List<InternshipSpecialization> InternshipSpecializations { get; set; } = [];
}

internal class UserInternshipConfiguration : IEntityTypeConfiguration<InternshipDetail>
{
    public void Configure(EntityTypeBuilder<InternshipDetail> builder)
    {
        builder.ToTable("InternshipDetails");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.InternshipDetails)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Specialization).IsRequired(true);

        builder.Property(x => x.ApprovedPoints).IsRequired(false);

        builder.Property(x => x.CompletedPoints).IsRequired(false);

        builder.Property(x => x.EngineerStage).IsRequired(true);

        builder.Property(x => x.StartDate).IsRequired(true);

        builder.Property(x => x.PlannedEndDate).IsRequired(true);

        builder.Property(x => x.Type).IsRequired(true).HasConversion<string>();
        builder.Property(x => x.TypeOrderWeight).HasComputedColumnSql(
            """CASE `Type` WHEN 'Primary' THEN 100 WHEN 'Extended' THEN 200 WHEN 'NDA' THEN 300 ELSE 400 END""",
            stored: true);

        builder.Property(x => x.FinalResult).IsRequired(false).HasConversion<string>();
        builder.Property(x => x.FinalResultOrderWeight).HasComputedColumnSql(
            """CASE `FinalResult` WHEN 'Success' THEN 100 WHEN 'Fired' THEN 200 ELSE 300 END""",
            stored: true);

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
