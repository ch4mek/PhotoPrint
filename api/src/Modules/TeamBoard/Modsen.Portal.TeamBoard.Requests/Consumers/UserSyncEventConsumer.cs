using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserSyncEventConsumer(
        ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery,
        IConnectionFactory connectionFactory)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
    {
        var @event = context.Message;

        var user = new CreateOrUpdateUserCoreDto
        {
            UserId = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            FirstNameRu = @event.FirstNameRu,
            LastNameRu = @event.LastNameRu,
            PatronymicRu = @event.PatronymicRu,
            DepartmentId = @event.DepartmentId,
            Email = @event.Email,
            Color = @event.Color,
            AvatarFileId = @event.AvatarFileId,
            Position = @event.Position,
            PositionOrderWeight = @event.PositionOrderWeight,
            Location = @event.Location?.ToString(),
            ResourceGroupId = @event.ResourceGroupId,
            RegisteredLocationId = @event.RegisteredLocationId,
            HireDate = @event.HireDate,
            CountryId = @event.CountryId,
            CurrentLocation = @event.CurrentLocation,
        };

        await _createOrUpdateUserDbQuery.Execute(user, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-teamboard-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
