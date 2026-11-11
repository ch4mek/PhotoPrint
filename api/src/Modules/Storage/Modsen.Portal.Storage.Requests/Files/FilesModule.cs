using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Storage.Requests.Files.Core.Composers;
using Modsen.Portal.Storage.Requests.Files.Core.Encryptor;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;
using Modsen.Portal.Storage.Requests.Files.Core.Loaders;

namespace Modsen.Portal.Storage.Requests.Files;

internal sealed class FilesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IUploadFileStreamComposer, UploadFileStreamComposer>();
        services.AddScoped<IPayslipEncryptor, PayslipEncryptor>();
        services.AddScoped<IFileStreamLoader, FileStreamLoader>();
        services.AddScoped<IApplicationImageResizeStorageService, ApplicationImageResizeStorageService>();
    }
}
