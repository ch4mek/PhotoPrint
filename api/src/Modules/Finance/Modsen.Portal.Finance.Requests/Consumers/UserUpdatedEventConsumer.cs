using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Core.DbQueries;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class UserUpdatedEventConsumer : IConsumer<UserUpdatedEvent>
{
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserUpdatedEventConsumer(ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery, IConnectionFactory connectionFactory)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
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

    internal sealed class UserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserUpdatedEventConsumer>("user-updated-finance-queue", "user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserUpdatedEventConsumer,
        UserUpdatedEventConsumerDefinition>;
}
