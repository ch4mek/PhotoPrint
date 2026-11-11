using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.GetUserHavingAnyTrackedTime;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.UserNames;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

public sealed class UsersDbQueriesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IGetUserHavingAnyTrackedTimeDbQuery, GetUserHavingAnyTrackedTimeDbQuery>();
        services.AddScoped<IGetUserNamesResponseByUserIdDbQuery, GetUserNamesResponseByUserIdDbQuery>();
        services.AddScoped<ICreateOrUpdateUserDbQuery, CreateOrUpdateUserDbQuery>();
        services.AddScoped<IUpdateIsActiveForUserDbQuery, UpdateIsActiveForUserDbQuery>();
        services.AddScoped<IGetUserDetailsDbQuery, GetUserDetailsDbQuery>();
        services.AddScoped<IIsTimeTrackUserExistingByUserIdDbQuery, IsTimeTrackUserExistingByUserIdDbQuery>();
        services.AddScoped<IGetTimeTrackUserByIdWithDepartmentDbQuery, GetTimeTrackUserByIdWithDepartmentDbQuery>();
    }
}

