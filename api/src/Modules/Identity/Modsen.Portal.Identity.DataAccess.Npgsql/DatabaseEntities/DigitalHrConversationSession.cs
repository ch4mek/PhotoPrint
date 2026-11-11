using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

public class DigitalHrConversationSession : AuditableEntity
{
    public required string ConversationId { get; set; }
    public required int UserId { get; set; }
    public required string Token { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public string? Watermark { get; set; }
}

internal class DigitalHrConversationSessionConfiguration : IEntityTypeConfiguration<DigitalHrConversationSession>
{
    public void Configure(EntityTypeBuilder<DigitalHrConversationSession> builder)
    {
        builder.ToTable("DigitalHrConversationSession", IdentityModuleDbContext.Schema);

        builder.HasKey(x => x.ConversationId);
        builder.Property(x => x.ConversationId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.Token).IsRequired();

        builder.HasIndex(x => x.UserId);
    }
}
