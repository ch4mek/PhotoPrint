using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class CvClientEntity : AuditableEntity
{
    public required string ClientId { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public ICollection<CvEntity> Cvs { get; set; } = [];
}

internal class CvClientConfiguration : IEntityTypeConfiguration<CvClientEntity>
{
    public void Configure(EntityTypeBuilder<CvClientEntity> builder)
    {
        builder.ToTable("CvClients");

        builder.HasKey(c => c.ClientId);
        builder.Property(c => c.ClientId)
            .ValueGeneratedNever();
        builder.Property(c => c.ClientId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired(false);
    }
}
