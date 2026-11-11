using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

public class CustomDetails
{
    public int Id { get; set; }
    public required string ReferenceId { get; set; }
    public required string ReferenceType { get; set; }
    public required string Value { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public MessageSource MessageSource { get; set; }
}

internal class CustomDetailsConfiguration : IEntityTypeConfiguration<CustomDetails>
{
    public void Configure(EntityTypeBuilder<CustomDetails> builder)
    {
        builder.ToTable("CustomDetails", IdentityModuleDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ReferenceId).IsRequired().HasMaxLength(256);

        builder.Property(x => x.ReferenceType).IsRequired().HasMaxLength(256);

        builder.Property(x => x.Value).IsRequired().HasMaxLength(256);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => new { x.ReferenceType, x.ReferenceId });
    }
}
