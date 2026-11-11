using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Outbox;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.DataAccess.Npgsql.Builders;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.DataAccess.Npgsql.Guards;
using Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.Identity.DataAccess.Npgsql;

public sealed class IdentityDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<IdentityModuleDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            optionsBuilder.UsePortalMySql(factory.GetConnection(), pgOptions =>
                {
                    pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, IdentityModuleDbContext.Schema);
                })
                .AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>())
                .ConfigureWarnings(warnings => warnings.Throw(
                    RelationalEventId.MultipleCollectionIncludeWarning,
                    CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IIdentityDbContext>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<IdentityModuleDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return context;
        });

        services.AddScoped<IIdentityQueryBuilder, IdentityQueryBuilder>();

        services.AddScoped<IDepartmentsCache, DepartmentsCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IDepartmentsCache>());
        services.AddScoped<IRedirectRulesCache, RedirectRulesCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IRedirectRulesCache>());
        services.AddScoped<IRolesCache, RolesCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IRolesCache>());
        services.AddScoped<IMicrofrontendsCache, MicrofrontendsCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IMicrofrontendsCache>());

        services.AddScoped<IDataConsistencyGuard, CeoCtoHeadConsistencyGuard>();

        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, ClaimGroupsSeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, ClaimsSeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, RolesSeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, DepartmentsSeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, UsersSeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, UserDepartmentHistorySeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, PermissionsSeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, RedirectRuleSeed>();
        services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, PortalSettingsSeed>();

        services.AddTransient<ISeedClaimProvider<IdentityModuleDbContext, Department>, DepartmentClaimsSeed>();
        services.AddTransient<ISeedClaimProvider<IdentityModuleDbContext, Role>, RoleClaimsSeed>();
        services.AddTransient<ISeedClaimProvider<IdentityModuleDbContext, User>, UserClaimSeed>();

        services.AddTransient<ISeedClaimProvider<IdentityModuleDbContext, Department>, DepartmentClaimsSeed>();
        services.AddTransient<ISeedClaimProvider<IdentityModuleDbContext, Role>, RoleClaimsSeed>();

        services
            .AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<IdentityModuleDbContext>();

        services.AddScoped<IPortalSettingsAccessor, PortalSettingCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IPortalSettingsAccessor>());

        services.AddOutboxRepository<IdentityModuleDbContext>();
    }
}
