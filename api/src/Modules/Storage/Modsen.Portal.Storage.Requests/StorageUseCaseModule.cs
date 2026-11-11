using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Requests.Consumers;
using Modsen.Portal.Storage.Requests.Files;
using Modsen.Portal.Storage.Requests.Jobs;

namespace Modsen.Portal.Storage.Requests;

public sealed class StorageUseCaseModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<FilesModule>();
        base.Load(services);

        services.AddScoped<IStorageContract, StorageContract>();
        services.AddScoped<IStorageCrossWriterContract, StorageCrossWriterContract>();

        services.AddScoped<BackfillApplicationImageVariantsJob>();
        services.AddScoped<ApplicationFileVariantsResizeJob>();
        services.AddScoped<CreateAvatarSmallSquaresJob>();
        services.AddScoped<IApplicationFileVariantsResizeJobDispatcher, ApplicationFileVariantsResizeJobDispatcher>();
    }

    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new DeleteFileNotificationEventConsumer.RegistrationDefinition();
        yield return new ApplicationFileUploadedNotificationEventConsumer.RegistrationDefinition();
    }
}
