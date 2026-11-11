using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;

public static class DbMappersExtensions
{
    internal static IServiceCollection AddDbMappers(this IServiceCollection services)
    {
        services.AddScoped<IPageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper,
                           PageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper>();
        services.AddScoped<IPageableOdpTeamboardUserToPageableOdpUserPerformanceReviewDtoMapper,
                           PageableOdpTeamboardUserToPageableOdpUserPerformanceReviewDtoMapper>();
        services.AddScoped<IPageableOdpLabBoardInternToPageableOdpLabBoardInternDtoMapper,
                           PageableOdpLabBoardInternToPageableOdpLabBoardInternDtoMapper>();

        return services;
    }
}
