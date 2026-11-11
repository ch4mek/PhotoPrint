using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Equipment.Requests.Consumers;

internal sealed class UserActivatedEventConsumer : IConsumer<UserActivatedEvent>
{
    private readonly EquipmentDbContext _equipmentDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserActivatedEventConsumer(
        IConnectionFactory connectionFactory,
        EquipmentDbContext equipmentDbContext)
    {
        _connectionFactory = connectionFactory;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task Consume(ConsumeContext<UserActivatedEvent> context)
    {
        var @event = context.Message;

        await ActivateUserAsync(@event.UserId, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private async Task ActivateUserAsync(int userId, CancellationToken cancellationToken)
    {
        var deletedUser = await _equipmentDbContext.EquipmentUsers
            .FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);

        if (deletedUser is not null)
        {
            deletedUser.IsActive = true;

            await _equipmentDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    internal sealed class UserActivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserActivatedEventConsumer>("user-activated-equipment-queue", "user-activated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserActivatedEventConsumer,
        UserActivatedEventConsumerDefinition>;
}
