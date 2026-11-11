using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class ArticleEntity : AuditableEntity
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public GroupEntity? Group { get; set; }

    public required string Name { get; set; }
    public string? Icon { get; set; }
    public int Views { get; set; }
    public required string Text { get; set; }
    public required string Keywords { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailableOnMainScreen { get; set; }
    public int OrderWeight { get; set; }

    public KnowledgeBaseUserEntity? CreatedByUser { get; set; }
    public KnowledgeBaseUserEntity? UpdatedByUser { get; set; }

    public List<ArticleReferenceEntity> ArticleReferences { get; set; } = [];
    public List<ArticleTagEntity> ArticleTags { get; set; } = [];
}

internal class ArticleConfiguration : IEntityTypeConfiguration<ArticleEntity>
{
    public void Configure(EntityTypeBuilder<ArticleEntity> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Icon).IsRequired(false).HasMaxLength(255);

        builder.Property(x => x.Views).IsRequired();

        builder.Property(x => x.Text).IsRequired();

        builder.Property(x => x.Keywords).IsRequired();

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.IsAvailableOnMainScreen).IsRequired();

        builder.Property(x => x.OrderWeight).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.GroupId).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
