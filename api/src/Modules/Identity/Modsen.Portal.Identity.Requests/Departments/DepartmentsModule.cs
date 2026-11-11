using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Requests.Departments.Core;

namespace Modsen.Portal.Identity.Requests.Departments;

internal sealed class DepartmentsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IDepartmentLoader, DepartmentLoader>();
    }
}
