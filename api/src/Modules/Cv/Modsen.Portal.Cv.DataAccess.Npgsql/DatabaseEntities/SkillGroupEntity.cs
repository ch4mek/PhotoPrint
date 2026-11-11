using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class SkillGroupEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string NameRu { get; set; }
    public bool IsCustom { get; set; }
    public int OrderWeight { get; set; }
    public ICollection<SkillEntity> Skills { get; set; } = [];
    public int CvId { get; set; }
    public CvEntity? Cv { get; set; }
}

internal class SkillGroupConfiguration : IEntityTypeConfiguration<SkillGroupEntity>
{
    public void Configure(EntityTypeBuilder<SkillGroupEntity> builder)
    {
        builder.ToTable("SkillGroups");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(s => s.NameRu)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(s => s.IsCustom)
            .IsRequired();

        builder.Property(s => s.OrderWeight)
            .IsRequired();

        builder.Property(s => s.CreatedAtUtc)
            .IsRequired();

        builder.Property(s => s.UpdatedAtUtc)
            .IsRequired(false);

        builder.HasOne(s => s.Cv)
            .WithMany(c => c.SkillGroups)
            .HasForeignKey(s => s.CvId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
