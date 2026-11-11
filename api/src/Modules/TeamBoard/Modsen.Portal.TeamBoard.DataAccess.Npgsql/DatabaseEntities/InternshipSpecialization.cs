using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class InternshipSpecialization : IPortalEntity
{
    public int Id { get; set; }

    public int InternshipDetailId { get; set; }
    public InternshipDetail? InternshipDetail { get; set; }

    public int SpecializationTemplateId { get; set; }
    public SpecializationTemplate? SpecializationTemplate { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<InternshipSpecializationHardSkill> InternshipSpecializationHardSkills { get; set; } = [];
}

internal class InternsipSpecializationConfiguration : IEntityTypeConfiguration<InternshipSpecialization>
{
    public void Configure(EntityTypeBuilder<InternshipSpecialization> builder)
    {
        builder.ToTable("InternshipSpecializations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => new
        {
            x.InternshipDetailId,
            x.SpecializationTemplateId
        }).IsUnique();

        builder.HasOne(x => x.InternshipDetail)
            .WithMany(x => x.InternshipSpecializations)
            .HasForeignKey(x => x.InternshipDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SpecializationTemplate)
            .WithMany(x => x.InternshipSpecializations)
            .HasForeignKey(x => x.SpecializationTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
