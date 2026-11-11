using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class GroupEntity : AuditableEntity
{
    public int Id { get; set; }

    public int? ParentGroupId { get; set; }
    public GroupEntity? ParentGroup { get; set; }
    public List<GroupEntity> ChildGroups { get; set; } = [];

    public required string Name { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsAvailableOnMainScreen { get; set; }
    public int OrderWeight { get; set; }

    public KnowledgeBaseUserEntity? CreatedByUser { get; set; }
    public KnowledgeBaseUserEntity? UpdatedByUser { get; set; }

    public List<ArticleEntity> Articles { get; set; } = [];
}

internal class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Icon).IsRequired(false).HasMaxLength(512);

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.IsDeleted).IsRequired();

        builder.Property(x => x.IsAvailableOnMainScreen).IsRequired();

        builder.Property(x => x.OrderWeight).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentGroup)
            .WithMany(x => x.ChildGroups)
            .HasForeignKey(x => x.ParentGroupId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Articles)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
