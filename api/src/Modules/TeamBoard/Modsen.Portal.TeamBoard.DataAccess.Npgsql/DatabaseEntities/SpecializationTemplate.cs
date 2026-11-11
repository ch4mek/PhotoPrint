using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class SpecializationTemplate : IAuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public List<SpecializationTemplateDepartment> SpecializationTemplateDepartments { get; set; } = [];
    public List<SpecializationTemplateHardSkill> SpecializationTemplateHardSkills { get; set; } = [];
    public List<UserSpecialization> UserSpecializations { get; set; } = [];
    public List<InternshipSpecialization> InternshipSpecializations { get; set; } = [];
}

internal class SpecializationTemplateConfiguration : IEntityTypeConfiguration<SpecializationTemplate>
{
    public void Configure(EntityTypeBuilder<SpecializationTemplate> builder)
    {
        builder.ToTable("SpecializationTemplates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();

        builder.Property(x => x.IsActive).IsRequired();

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
