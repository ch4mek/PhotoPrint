using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal class MerchUserEntity
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public bool IsActive { get; set; }
}

internal class MerchUserConfiguration : IEntityTypeConfiguration<MerchUserEntity>
{
    public void Configure(EntityTypeBuilder<MerchUserEntity> builder)
    {
        builder.ToTable("MerchUsers");

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.LastName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Email).IsRequired().HasMaxLength(PortalSharedConstants.EmailMaxLength);

        builder.Property(x => x.DepartmentId).IsRequired(false).HasMaxLength(64);

        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
    }
}
