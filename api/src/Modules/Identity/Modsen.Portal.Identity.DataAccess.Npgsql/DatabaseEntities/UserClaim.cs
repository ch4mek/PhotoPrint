using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class UserClaim : IdentityUserClaim<int>
{
    public ClaimEntity? Claim { get; set; }
    public User? User { get; set; }
}

internal class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaims");

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.UserId);

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
