using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class ConversationDocumentLink
{
    public int Id { get; set; }
    public required string Link { get; set; }
    public int ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
}

internal class ConversationDocumentLinkConfiguration : IEntityTypeConfiguration<ConversationDocumentLink>
{
    public void Configure(EntityTypeBuilder<ConversationDocumentLink> builder)
    {
        builder.ToTable("ConversationDocumentLinks");

        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).ValueGeneratedOnAdd();

        builder.Property(entity => entity.Link).IsRequired();

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.ConversationDocumentLinks)
            .HasForeignKey(x => x.ConversationId);
    }
}

