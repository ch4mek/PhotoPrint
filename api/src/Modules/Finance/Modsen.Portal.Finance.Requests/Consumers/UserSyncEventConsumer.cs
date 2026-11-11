using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Core.DbQueries;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class UserSyncEventConsumer : IConsumer<UserSyncEvent>
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

        if (@event.Position.IsIntern())
        {
            return;
        }

        var user = new CreateOrUpdateUserCoreDto
        {
            Id = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            FirstNameRu = @event.FirstNameRu,
            LastNameRu = @event.LastNameRu,
            PatronymicRu = @event.PatronymicRu,
            DepartmentId = @event.DepartmentId,
            Email = @event.Email,
            ResourceGroupId = @event.ResourceGroupId,
        };

        await _createOrUpdateUserDbQuery.Execute(user, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-finance-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
