using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class SpecializationTemplateDepartment : IPortalEntity
{
    public required string DepartmentId { get; set; }
    public int SpecializationTemplateId { get; set; }
    public SpecializationTemplate? SpecializationTemplate { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

internal class SpecializationTemplateDepartmentConfiguration :
    IEntityTypeConfiguration<SpecializationTemplateDepartment>
{
    public void Configure(EntityTypeBuilder<SpecializationTemplateDepartment> builder)
    {
        builder.ToTable("SpecializationTemplateDepartments");

        builder.HasKey(x => new
        {
            x.SpecializationTemplateId,
            x.DepartmentId
        });

        builder.HasOne(x => x.SpecializationTemplate)
            .WithMany(x => x.SpecializationTemplateDepartments)
            .HasForeignKey(x => x.SpecializationTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
