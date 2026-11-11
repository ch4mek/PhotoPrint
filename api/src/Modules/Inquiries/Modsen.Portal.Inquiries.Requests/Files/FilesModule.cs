using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.Files.Core.BusinessRules;

namespace Modsen.Portal.Inquiries.Requests.Files;

internal sealed class FilesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IHasUserAccessToFilesBusinessRule, HasUserAccessToFilesBusinessRule>();
    }
}
