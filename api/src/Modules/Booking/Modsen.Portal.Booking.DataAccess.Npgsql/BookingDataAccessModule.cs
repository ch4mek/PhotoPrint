using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.DataAccess.Npgsql.Seeds;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Booking.DataAccess.Npgsql;

public sealed class BookingDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<BookingDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();
            optionsBuilder.UseNpgsql(factory.GetConnection(), pgOptions =>
                {
                    pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, BookingDbContext.Schema);
                })
                .AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>())
                .ConfigureWarnings(warnings => warnings.Throw(
                    RelationalEventId.MultipleCollectionIncludeWarning,
                    CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IBookingDbContext>(sp =>
        {
            var context = sp.GetRequiredService<BookingDbContext>();
            var connectionFactory = sp.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return context;
        });

        services.AddTransient<ISeedEntitiesProvider<BookingDbContext>, OfficeSeed>();
    }
}
