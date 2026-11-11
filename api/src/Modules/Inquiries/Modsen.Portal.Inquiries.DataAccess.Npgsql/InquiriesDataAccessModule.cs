using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql;

public sealed class InquiriesDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<InquiriesDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            options.UseNpgsql(factory.GetConnection(), pgOptions =>
            {
                pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, InquiriesDbContext.Schema);
                pgOptions.MigrationsAssembly(typeof(InquiriesDataAccessModule).Assembly.FullName);
            });
            options.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
            options.ConfigureWarnings(warnings => warnings.Throw(
                RelationalEventId.MultipleCollectionIncludeWarning,
                CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IInquiriesDbContext>(serviceProvider =>
        {
            var inquiriesDbContext = serviceProvider.GetRequiredService<InquiriesDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            inquiriesDbContext.Database.UseTransaction(connectionFactory.GetTransaction());

            return inquiriesDbContext;
        });

        services.AddTransient<ISeedEntitiesProvider<InquiriesDbContext>, InquiryTemplatesSeed>();
        services.AddTransient<ISeedEntitiesProvider<InquiriesDbContext>, CalendarExclusionsSeed>();
    }
}
