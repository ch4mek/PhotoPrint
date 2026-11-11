using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class LanguageEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public CvLanguageLevel Level { get; set; }
    public int CvId { get; set; }
    public CvEntity? Cv { get; set; }
}

internal class LanguageConfiguration : IEntityTypeConfiguration<LanguageEntity>
{
    public void Configure(EntityTypeBuilder<LanguageEntity> builder)
    {
        builder.ToTable("Languages");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(l => l.Level)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(l => l.CreatedAtUtc)
            .IsRequired();

        builder.Property(l => l.UpdatedAtUtc)
            .IsRequired(false);

        builder.HasOne(l => l.Cv)
            .WithMany(c => c.Languages)
            .HasForeignKey(l => l.CvId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
