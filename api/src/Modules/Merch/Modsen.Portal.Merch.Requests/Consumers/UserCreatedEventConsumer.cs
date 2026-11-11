using Amazon.RDS.Model;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Consumers;

internal sealed class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IMerchDbContext _merchDbContext;
    private readonly IConnectionFactory _connectionFactory;
    public UserCreatedEventConsumer(IMerchDbContext merchDbContext,
        IConnectionFactory connectionFactory)
    {
        _merchDbContext = merchDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        if (context.Message.Position.IsIntern())
        {
            return;
        }

        var user = await _merchDbContext.MerchUsers
            .Where(x => x.UserId == context.Message.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (user == null)
        {
            user = new MerchUserEntity
            {
                UserId = context.Message.UserId,
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName,
                Email = context.Message.Email,
                DepartmentId = context.Message.DepartmentId,
                IsActive = true
            };

            _merchDbContext.MerchUsers.Add(user);
            await _merchDbContext.SaveChangesAsync(context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserCreatedEventConsumer>("user-created-merch-queue", "user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserCreatedEventConsumer,
        UserCreatedEventConsumerDefinition>;
}
