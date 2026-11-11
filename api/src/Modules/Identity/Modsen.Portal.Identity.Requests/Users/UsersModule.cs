using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Core.Validators;
using Modsen.Portal.Identity.Requests.Users.Requests.Queries;

namespace Modsen.Portal.Identity.Requests.Users;

internal sealed class UsersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IUserLookupLoader, UserLookupLoader>();

        services.AddScoped<ICanCurrentUserCreateUserWithRoleBusinessRule, CanCurrentUserCreateUserWithRoleBusinessRule>();
        services.AddScoped<ICanIdentityUserBeActivatedBusinessRule, CanIdentityUserBeActivatedBusinessRule>();
        services.AddScoped<ICanRoleBeOccupiedBusinessRule, CanRoleBeOccupiedBusinessRule>();
        services.AddScoped<ICanRoleHaveDepartmentBusinessRule, CanRoleHaveDepartmentBusinessRule>();
        services.AddScoped<ICanUserBeProjectManagerByDepartmentIdBusinessRule, CanUserBeProjectManagerByDepartmentIdBusinessRule>();
        services.AddScoped<ICanUserFieldsBeUpdatedBusinessRule, CanUserFieldsBeUpdatedBusinessRule>();
        services.AddScoped<IIsApplicationRolesCorrectBusinessRule, IsApplicationRolesCorrectBusinessRule>();
        services.AddScoped<IIsManagerUserUpdateValidBusinessRule, IsManagerUserUpdateValidBusinessRule>();
        services.AddScoped<IIsUserManagerBusinessRule, IsUserManagerBusinessRule>();
        services.AddScoped<ICanUserWithResourceGroupBeCreatedBusinessRule, CanUserWithResourceGroupBeCreatedBusinessRule>();
        services.AddScoped<ICanUserBeUpdatedBusinessRule, CanUserBeUpdatedBusinessRule>();

        services.AddScoped<IUpdateUserValidator, UpdateUserValidator>();

        services.AddScoped<IGetGroupByIdDbQuery, GetGroupByIdDbQuery>();
        services.AddScoped<IUserDepartmentHistoryComposer, UserDepartmentHistoryComposer>();
    }
}
