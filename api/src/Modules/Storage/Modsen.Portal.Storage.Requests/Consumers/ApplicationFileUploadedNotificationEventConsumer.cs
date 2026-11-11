using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Storage.Contract.Events;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;
using Modsen.Portal.Storage.Requests.Jobs;

namespace Modsen.Portal.Storage.Requests.Consumers;

internal sealed class ApplicationFileUploadedNotificationEventConsumer(
    IStorageDbContext dbContext,
    IApplicationFileVariantsResizeJobDispatcher jobDispatcher)
    : IConsumer<ApplicationFileUploadedEvent>
{
    public async Task Consume(ConsumeContext<ApplicationFileUploadedEvent> context)
    {
        var file = await dbContext.ApplicationFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FileId == context.Message.FileId, context.CancellationToken);

        if (file is null)
        {
            throw new EntityNotFoundException("File does not exist");
        }

        if (!ImageCompressionPolicy.IsResizableImage(file.ContentType))
        {
            return;
        }

        var targetSizes = ImageCompressionPolicy.GetTargetSizes(context.Message.FilePurpose);

        if (targetSizes.Count == 0)
        {
            return;
        }

        jobDispatcher.Dispatch(context.Message);
    }

    internal sealed class ApplicationFileUploadedNotificationEventConsumerDefinition() :
        ConsumerDefinitionBase<ApplicationFileUploadedNotificationEventConsumer>(
            "application-file-uploaded-storage-queue",
            "application-file-uploaded-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ApplicationFileUploadedNotificationEventConsumer,
        ApplicationFileUploadedNotificationEventConsumerDefinition>;
}
