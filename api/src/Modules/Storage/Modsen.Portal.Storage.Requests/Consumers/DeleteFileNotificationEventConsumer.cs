using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Storage.Contract;

namespace Modsen.Portal.Storage.Requests.Consumers;

internal sealed class DeleteFileNotificationEventConsumer : IConsumer<DeleteFileEvent>
{
    private readonly IStorageContract _storageContract;

    public DeleteFileNotificationEventConsumer(IStorageContract storageContract)
    {
        _storageContract = storageContract;
    }

    public Task Consume(ConsumeContext<DeleteFileEvent> context)
    {
        return _storageContract.DeleteFile(context.Message.FileId, context.Message.FileSource, context.CancellationToken);
    }

    internal sealed class DeleteFileNotificationEventConsumerDefinition() :
        ConsumerDefinitionBase<DeleteFileNotificationEventConsumer>(
            "delete-file-storage-queue",
            "delete-file-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        DeleteFileNotificationEventConsumer,
        DeleteFileNotificationEventConsumerDefinition>;
}
