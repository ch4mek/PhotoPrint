using Microsoft.EntityFrameworkCore;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

internal class KnowledgeBaseDbContext : DbContext, IKnowledgeBaseDbContext
{
    public const string Schema = "knowledgebase";

    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<ArticleEntity> Articles { get; set; }
    public DbSet<ArticleReferenceEntity> ArticleReferences { get; set; }
    public DbSet<DiscountCategoryEntity> DiscountCategories { get; set; }
    public DbSet<DiscountEntity> Discounts { get; set; }
    public DbSet<DiscountReferenceEntity> DiscountsReferences { get; set; }
    public DbSet<CityEntity> Cities { get; set; }
    public DbSet<DiscountCityEntity> DiscountCities { get; set; }
    public DbSet<TagEntity> Tags { get; set; }
    public DbSet<DiscountTagEntity> DiscountTags { get; set; }
    public DbSet<DiscountCategoryTagEntity> DiscountCategoryTags { get; set; }
    public DbSet<ArticleTagEntity> ArticleTags { get; set; }
    public DbSet<KnowledgeBaseUserEntity> KnowledgeBaseUsers { get; set; }

    public KnowledgeBaseDbContext(DbContextOptions<KnowledgeBaseDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KnowledgeBaseDbContext).Assembly);
    }
}
