using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class RoleClaim : IdentityRoleClaim<int>
{
    public ClaimEntity? Claim { get; set; }
    public Role? Role { get; set; }
}

internal class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims");
        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.RoleId);

        builder
            .HasOne(dc => dc.Claim)
            .WithMany()
            .HasForeignKey(dc => dc.ClaimType)
            .HasPrincipalKey(c => c.ClaimType)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ClaimType).HasMaxLength(256).IsRequired();
        builder.Property(x => x.ClaimValue).HasMaxLength(256).IsRequired();
    }
}
