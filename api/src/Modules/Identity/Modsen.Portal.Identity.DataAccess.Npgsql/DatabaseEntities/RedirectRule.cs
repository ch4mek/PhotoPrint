using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class RedirectRule
{
    public Guid Id { get; set; }
    public required string RedirectKey { get; set; }
    public required string RedirectTo { get; set; }
}

internal class RedirectRuleConfiguration : IEntityTypeConfiguration<RedirectRule>
{
    public void Configure(EntityTypeBuilder<RedirectRule> builder)
    {
        builder.ToTable("RedirectRules", IdentityModuleDbContext.Schema);

        builder.HasKey(redirectRule => redirectRule.Id);

        builder.Property(redirectRule => redirectRule.RedirectKey).IsRequired().HasMaxLength(128);
        builder.HasIndex(redirectRule => redirectRule.RedirectKey).IsUnique();

        builder.Property(redirectRule => redirectRule.RedirectTo).IsRequired().HasMaxLength(4096);
    }
}
