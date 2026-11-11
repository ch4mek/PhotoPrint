using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class ClaimGroup
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public List<ClaimEntity> Claims { get; set; } = [];
}

internal class ClaimGroupConfiguration : IEntityTypeConfiguration<ClaimGroup>
{
    public void Configure(EntityTypeBuilder<ClaimGroup> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.Name).HasMaxLength(256);
        builder.Property(x => x.Id).HasMaxLength(64);

        builder.ToTable("ClaimGroups", IdentityModuleDbContext.Schema);

        builder
            .HasMany(group => group.Claims)
            .WithOne(claim => claim.Group)
            .HasForeignKey(claim => claim.GroupId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}
