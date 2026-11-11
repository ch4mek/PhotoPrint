using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly IMerchDbContext _merchDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserSyncEventConsumer(IMerchDbContext merchDbContext,
        IConnectionFactory connectionFactory)
    {
        _merchDbContext = merchDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
    {
        var @event = context.Message;

        if (context.Message.Position.IsIntern())
        {
            return;
        }

        var user = await _merchDbContext.MerchUsers
            .Where(x => x.UserId == @event.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (user is null)
        {
            user = new MerchUserEntity
            {
                UserId = @event.UserId,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                Email = @event.Email
            };

            _merchDbContext.MerchUsers.Add(user);
        }

        user.FirstName = @event.FirstName;
        user.LastName = @event.LastName;
        user.Email = @event.Email;
        user.DepartmentId = @event.DepartmentId!;

        await _merchDbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);   
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-merch-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
