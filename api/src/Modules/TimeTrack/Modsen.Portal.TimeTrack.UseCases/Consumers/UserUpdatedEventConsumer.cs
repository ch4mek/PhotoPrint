using MassTransit;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Consumers;

internal sealed class UserUpdatedEventConsumer : IConsumer<UserUpdatedEvent>
{
    private readonly IRemoveProjectManagerByUserIdDbQuery _removeProjectManagerByUserIdDbQuery;
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly ILogger<UserUpdatedEventConsumer> _logger;
    private readonly ITimeTrackUserProvider _timeTrackUserProvider;
    private readonly ITimeTrackPortalSettingsAccessor _timeTrackPortalSettingsAccessor;
    private readonly IConnectionFactory _connectionFactory;

    public UserUpdatedEventConsumer(
        ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery,
        ILogger<UserUpdatedEventConsumer> logger,
        IRemoveProjectManagerByUserIdDbQuery removeProjectManagerByUserIdDbQuery,
        ITimeTrackUserProvider timeTrackUserProvider,
        ITimeTrackPortalSettingsAccessor timeTrackPortalSettingsAccessor,
        IConnectionFactory connectionFactory)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _logger = logger;
        _removeProjectManagerByUserIdDbQuery = removeProjectManagerByUserIdDbQuery;
        _timeTrackUserProvider = timeTrackUserProvider;
        _timeTrackPortalSettingsAccessor = timeTrackPortalSettingsAccessor;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation("[TimeTrack] Received User Updated Event with ID {EventId}. User Id: {UserId}",
            @event.EventId,
            @event.UserId);

        var user = await _timeTrackUserProvider.GetByIdAsync(@event.UserId, context.CancellationToken);

        if (user != null)
        {
            var settings = await _timeTrackPortalSettingsAccessor.GetSettings(context.CancellationToken);

            if (user.DepartmentId == settings.PmoDepartmentId &&
                @event.DepartmentId != settings.PmoDepartmentId)
            {
                await _removeProjectManagerByUserIdDbQuery.Execute(@event.UserId, context.CancellationToken);
            }
        }

        var updatedUser = new TimeTrackUser
        {
            UserId = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            DepartmentId = @event.DepartmentId,
            Email = @event.Email,
            Color = @event.Color,
            AvatarFileId = @event.AvatarFileId,
            ResourceGroupId = @event.ResourceGroupId
        };

        await _createOrUpdateUserDbQuery.Execute(updatedUser, @event.Position, context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);

        _logger.LogInformation("Completed User Updated Event with ID {EventId}", @event.EventId);
    }

    internal sealed class UserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserUpdatedEventConsumer>("user-updated-timetrack-queue", "user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserUpdatedEventConsumer,
        UserUpdatedEventConsumerDefinition>;
}
