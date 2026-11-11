using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Outbox;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql;

public sealed class TeamBoardDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<TeamBoardDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            options.UsePortalMySql(factory.GetConnection(), pgOptions =>
            {
                pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, TeamBoardDbContext.Schema);
                pgOptions.MigrationsAssembly(typeof(TeamBoardDataAccessModule).Assembly.FullName);
            });
            options.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
            options.ConfigureWarnings(warnings => warnings.Throw(
                RelationalEventId.MultipleCollectionIncludeWarning,
                CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<ITeamBoardDbContext>(serviceProvider =>
        {
            var teamBoardDbContext = serviceProvider.GetRequiredService<TeamBoardDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            teamBoardDbContext.Database.UseTransaction(connectionFactory.GetTransaction());

            return teamBoardDbContext;
        });

        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, CountriesSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, CurrenciesSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, EnglishLevelsSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, SeniorityLevelsSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, WorkingHoursPerMonthsSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, HardSkillsSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, SoftSkillsSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, KpiColorsSeed>();

        //remove after applying
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, UserCareerStartedAtSeed>();
        services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, UserContractSeed>();

        services.AddScoped<IHardSkillGroupCache, HardSkillGroupsCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IHardSkillGroupCache>());
        services.AddScoped<ISeniorityLevelCache, SeniorityLevelCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<ISeniorityLevelCache>());
        services.AddScoped<IEnglishLevelCache, EnglishLevelCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IEnglishLevelCache>());

        services.AddOutboxRepository<TeamBoardDbContext>();

        services.AddDbMappers();
    }
}
