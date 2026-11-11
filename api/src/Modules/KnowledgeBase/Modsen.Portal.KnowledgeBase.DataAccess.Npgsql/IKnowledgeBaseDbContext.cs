using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.KnowledgeBase.Requests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Tools.DiscountsMigrator")]
namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

internal interface IKnowledgeBaseDbContext
{
    public DbSet<GroupEntity> Groups { get; }
    public DbSet<ArticleEntity> Articles { get; }
    public DbSet<ArticleReferenceEntity> ArticleReferences { get; }
    public DbSet<DiscountCategoryEntity> DiscountCategories { get; }
    public DbSet<DiscountEntity> Discounts { get; }
    public DbSet<DiscountReferenceEntity> DiscountsReferences { get; }
    public DbSet<CityEntity> Cities { get; }
    public DbSet<DiscountCityEntity> DiscountCities { get; }
    public DbSet<TagEntity> Tags { get; }
    public DbSet<DiscountCategoryTagEntity> DiscountCategoryTags { get; }
    public DbSet<DiscountTagEntity> DiscountTags { get; }
    public DbSet<ArticleTagEntity> ArticleTags { get; }
    public DbSet<KnowledgeBaseUserEntity> KnowledgeBaseUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
