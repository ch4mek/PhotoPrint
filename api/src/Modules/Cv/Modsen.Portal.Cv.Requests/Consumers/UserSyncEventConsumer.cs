using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.Requests.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Core.DbQueries;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Cv.Requests.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;

    public UserSyncEventConsumer(
        IConnectionFactory connectionFactory,
        ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery)
    {
        _connectionFactory = connectionFactory;
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
    {
        var user = new CreateOrUpdateUserCoreDto
        {
            UserId = context.Message.UserId,
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            FirstNameRu = context.Message.FirstNameRu,
            LastNameRu = context.Message.LastNameRu,
            Email = context.Message.Email,
            DepartmentId = context.Message.DepartmentId,
            ResourceGroupId = context.Message.ResourceGroupId,
            Position = context.Message.Position,
        };

        await _createOrUpdateUserDbQuery.Execute(user, context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-cv-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
