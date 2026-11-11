using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Composers;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.DbQueries;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Loaders;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments;

internal sealed class PermissionAssignmentsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICanUserAccessResourcesBusinessRule, CanUserAccessResourcesBusinessRule>();

        services.AddScoped<IPermissionAssignmentsComposer, PermissionAssignmentsComposer>();

        services.AddScoped<IPermissionAssignmentsLoader, PermissionAssignmentsLoader>();

        services.AddScoped<IDeletePermissionAssignmentsDbQuery, DeletePermissionAssignmentsDbQuery>();
        services.AddScoped<IGetPermissionAssignmentByIdDbQuery, GetPermissionAssignmentByIdDbQuery>();
    }
}
