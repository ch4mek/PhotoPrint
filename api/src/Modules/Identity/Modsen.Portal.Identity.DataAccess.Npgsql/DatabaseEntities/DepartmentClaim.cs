using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class DepartmentClaim
{
    public int Id { get; set; }
    public required string DepartmentId { get; set; }
    public Department? Department { get; set; }

    public required string ClaimType { get; set; }
    public ClaimEntity? Claim { get; set; }
    public required string ClaimValue { get; set; }

    public Claim ToClaim()
    {
        return new Claim(ClaimType, ClaimValue);
    }
}

internal class DepartmentClaimConfiguration : IEntityTypeConfiguration<DepartmentClaim>
{
    public void Configure(EntityTypeBuilder<DepartmentClaim> builder)
    {
        builder.ToTable("DepartmentClaims", IdentityModuleDbContext.Schema);

        builder.HasKey(e => e.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.DepartmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.DepartmentId).HasMaxLength(64);

        builder.Property(x => x.ClaimType).IsRequired().HasMaxLength(256);

        builder.Property(x => x.ClaimValue).IsRequired().HasMaxLength(256);

        builder.HasOne(dc => dc.Claim)
            .WithMany()
            .HasForeignKey(dc => dc.ClaimType)
            .HasPrincipalKey(c => c.ClaimType)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
