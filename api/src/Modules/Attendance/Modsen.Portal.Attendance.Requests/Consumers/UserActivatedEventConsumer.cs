using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal class UserActivatedEventConsumer : IConsumer<UserActivatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IAttendanceDbContext _dbContext;

    public UserActivatedEventConsumer(IConnectionFactory connectionFactory,
        IAttendanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserActivatedEvent> context)
    {
        var @event = context.Message;

        await _dbContext.Users
            .Where(user => user.UserId == @event.UserId)
            .ExecuteUpdateAsync(x => x.SetProperty(user => user.IsActive, true));

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserActivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserActivatedEventConsumer>("user-activated-attendance-queue", "user-activated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserActivatedEventConsumer,
        UserActivatedEventConsumerDefinition>;
}
