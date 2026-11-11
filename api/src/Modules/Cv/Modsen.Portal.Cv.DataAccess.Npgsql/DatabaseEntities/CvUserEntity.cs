using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class CvUserEntity : AuditableEntity
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? Position { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }

    public List<CvEntity> Cvs { get; set; } = [];
}

internal class CvUserEntityConfiguration : IEntityTypeConfiguration<CvUserEntity>
{
    public void Configure(EntityTypeBuilder<CvUserEntity> builder)
    {
        builder.ToTable("CvUsers");

        builder.HasKey(user => user.UserId);
        builder.Property(user => user.UserId)
            .ValueGeneratedNever();

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(user => user.FirstNameRu)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(user => user.LastNameRu)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(PortalSharedConstants.EmailMaxLength);

        builder.Property(user => user.DepartmentId)
            .IsRequired(false)
            .HasMaxLength(64);

        builder.Property(user => user.ResourceGroupId)
            .IsRequired(false);

        builder.Property(user => user.IsActive)
            .IsRequired()
            .HasDefaultValue(value: true);

        builder.Property(x => x.Position)
            .IsRequired(false)
            .HasMaxLength(64);

        builder.Property(x => x.Location)
            .IsRequired(false)
            .HasMaxLength(250);
    }
}
