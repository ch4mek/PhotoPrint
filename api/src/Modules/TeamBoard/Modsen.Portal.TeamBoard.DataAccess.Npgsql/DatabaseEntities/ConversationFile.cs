using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class ConversationFile
{
    public int Id { get; set; }

    public required string FileId { get; set; }
    public required string FileName { get; set; }
    public long FileSize { get; set; }

    public int ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

}

internal class ConversationFileConfiguration : IEntityTypeConfiguration<ConversationFile>
{
    public void Configure(EntityTypeBuilder<ConversationFile> builder)
    {
        builder.ToTable("ConversationFiles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FileId).IsRequired();
        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.FileSize).HasDefaultValue(0);
        builder.Property(x => x.ConversationId).IsRequired();

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.ConversationFiles)
            .HasForeignKey(x => x.ConversationId);
    }
}
