using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.GetWithTotalTrackedTime;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

public sealed class ProjectsDbQueriesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IGetProjectsWithTotalTrackedTimeDbQuery, GetProjectsWithTotalTrackedTimeDbQuery>();
        services.AddScoped<IGetProjectNameByProjectIdDbQuery, GetProjectNameByProjectIdDbQuery>();
        services.AddScoped<IRemoveProjectManagerByUserIdDbQuery, RemoveProjectManagerByUserIdDbQuery>();
        services.AddScoped<IIsProjectExistingByIdDbQuery, IsProjectExistingByIdDbQuery>();
        services.AddScoped<IGetProjectUsersByUserIdAndProjectIdDbQuery, GetProjectUsersByUserIdAndProjectIdDbQuery>();
        services.AddScoped<IUpdateProjectUserDbQuery, UpdateProjectUserDbQuery>();
        services.AddScoped<IGetUserProjectsWithDisabledDaysDbQuery, GetUserProjectsWithDisabledDaysDbQuery>();
        services.AddScoped<IGetProjectManagerIdsByUserIdDbQuery, GetProjectManagerIdsByUserIdDbQuery>();
        services.AddScoped<IGetProjectUserByIdDbQuery, GetProjectUserByIdDbQuery>();
    }
}
