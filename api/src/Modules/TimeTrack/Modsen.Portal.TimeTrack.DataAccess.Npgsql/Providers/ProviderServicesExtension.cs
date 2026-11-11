using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Providers;

internal static class ProviderServicesExtensions
{
    internal static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddScoped<IProjectsProvider, ProjectsProvider>();
        services.AddScoped<IProjectUsersProvider, ProjectUsersProvider>();
        services.AddScoped<ITimeTrackUserProvider, TimeTrackUserProvider>();

        return services;
    }
}
