using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Consumers;

internal sealed class UserUpdatedEventConsumer : IConsumer<UserUpdatedEvent>
{
    private readonly IMerchDbContext _merchDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserUpdatedEventConsumer(IMerchDbContext merchDbContext,
        IConnectionFactory connectionFactory)
    {
        _merchDbContext = merchDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        if (context.Message.Position.IsIntern())
        {
            return;
        }

        var user = await _merchDbContext.MerchUsers
            .Where(x => x.UserId == context.Message.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (user is null)
        {
            user = new MerchUserEntity
            {
                UserId = context.Message.UserId,
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName,
                Email = context.Message.Email
            };

            _merchDbContext.MerchUsers.Add(user);
        }

        user.FirstName = context.Message.FirstName;
        user.LastName = context.Message.LastName;
        user.Email = context.Message.Email;
        user.DepartmentId = context.Message.DepartmentId;

        await _merchDbContext.SaveChangesAsync(context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserUpdatedEventConsumer>("user-updated-merch-queue", "user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserUpdatedEventConsumer,
        UserUpdatedEventConsumerDefinition>;
}
