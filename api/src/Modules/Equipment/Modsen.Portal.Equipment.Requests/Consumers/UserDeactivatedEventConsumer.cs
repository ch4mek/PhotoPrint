using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Equipment.Requests.Consumers;

internal sealed class UserDeactivatedEventConsumer : IConsumer<UserDeactivatedEvent>
{
    private readonly IEquipmentDbContext _equipmentDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserDeactivatedEventConsumer(IConnectionFactory connectionFactory,
        IEquipmentDbContext equipmentDbContext)
    {
        _connectionFactory = connectionFactory;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task Consume(ConsumeContext<UserDeactivatedEvent> context)
    {
        var @event = context.Message;

        await DeactivateUserAsync(@event.UserId, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private async Task DeactivateUserAsync(int userId, CancellationToken cancellationToken)
    {
        var deletedUser = await _equipmentDbContext.EquipmentUsers
            .FirstOrDefaultAsync(user => user.UserId == userId,
                cancellationToken);

        if (deletedUser is not null)
        {
            deletedUser.IsActive = false;

            await _equipmentDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    internal sealed class UserDeactivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserDeactivatedEventConsumer>("user-deactivated-equipment-queue", "user-deactivated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserDeactivatedEventConsumer,
        UserDeactivatedEventConsumerDefinition>;
}
