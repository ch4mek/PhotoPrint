using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal class UserDeactivatedEventConsumer : IConsumer<UserDeactivatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly AttendanceDbContext _dbContext;

    public UserDeactivatedEventConsumer(IConnectionFactory connectionFactory,
        AttendanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserDeactivatedEvent> context)
    {
        var @event = context.Message;

        await _dbContext.Users
            .Where(user => user.UserId == @event.UserId)
            .ExecuteUpdateAsync(x => x.SetProperty(user => user.IsActive, false));

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserDeactivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserDeactivatedEventConsumer>("user-deactivated-attendance-queue", "user-deactivated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserDeactivatedEventConsumer,
        UserDeactivatedEventConsumerDefinition>;
}
