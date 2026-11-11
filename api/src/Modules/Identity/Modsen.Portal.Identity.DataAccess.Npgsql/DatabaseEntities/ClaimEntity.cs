using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class ClaimEntity
{
    public required string ClaimType { get; set; }
    public AllowedValueType AllowedValueType { get; set; }
    public int OrderWeight { get; set; }
    public ClaimGroup? Group { get; set; }
    public string? GroupId { get; set; }
}

internal class ClaimEntityConfiguration : IEntityTypeConfiguration<ClaimEntity>
{
    public void Configure(EntityTypeBuilder<ClaimEntity> builder)
    {
        builder.HasKey(x => x.ClaimType);
        builder.Property(x => x.ClaimType).ValueGeneratedNever();

        builder.Property(x => x.ClaimType).HasMaxLength(256).IsRequired();
        builder.Property(x => x.GroupId).HasMaxLength(64).IsRequired(false);

        builder
          .HasOne(x => x.Group)
          .WithMany(g=> g.Claims)
          .HasForeignKey(x => x.GroupId)
          .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Claims", IdentityModuleDbContext.Schema);

        builder
            .Property(x => x.AllowedValueType)
            .IsRequired()
            .HasConversion<string>();
    }
}

