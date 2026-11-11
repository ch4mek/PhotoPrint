using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

public sealed class KnowledgeBaseDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<KnowledgeBaseDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            options.UseNpgsql(factory.GetConnection(), pgOptions =>
            {
                pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, KnowledgeBaseDbContext.Schema);
                pgOptions.MigrationsAssembly(typeof(KnowledgeBaseDataAccessModule).Assembly.FullName);
            });
            options.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
            options.ConfigureWarnings(warnings => warnings.Throw(
                RelationalEventId.MultipleCollectionIncludeWarning,
                CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IKnowledgeBaseDbContext>(serviceProvider =>
        {
            var knowledgeBaseDbContext = serviceProvider.GetRequiredService<KnowledgeBaseDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            knowledgeBaseDbContext.Database.UseTransaction(connectionFactory.GetTransaction());

            return knowledgeBaseDbContext;
        });

        services.AddTransient<ISeedEntitiesProvider<KnowledgeBaseDbContext>, CitiesSeed>();
        services.AddTransient<ISeedEntitiesProvider<KnowledgeBaseDbContext>, DiscountsSeed>();

        services.AddTransient<ISeedEntitiesProvider<KnowledgeBaseDbContext>, ArticlesSeed>();
    }
}
