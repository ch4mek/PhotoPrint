using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.KnowledgeBase.Requests.Core.CoreDto;
using Modsen.Portal.KnowledgeBase.Requests.Core.DbQueries;

namespace Modsen.Portal.KnowledgeBase.Requests.Consumers;

internal sealed class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserCreatedEventConsumer(
        ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery,
        IConnectionFactory connectionFactory)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var user = new CreateOrUpdateUserCoreDto
        {
            UserId = context.Message.UserId,
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            DepartmentId = context.Message.DepartmentId,
            Email = context.Message.Email,
            Position = context.Message.Position,
        };

        await _createOrUpdateUserDbQuery.Execute(user, context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserCreatedEventConsumer>("user-created-knowledgebase-queue", "user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserCreatedEventConsumer,
        UserCreatedEventConsumerDefinition>;
}
