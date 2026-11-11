using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Booking.Requests.Consumers;

internal sealed class UserActivatedEventConsumer : IConsumer<UserActivatedEvent>
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserActivatedEventConsumer(IBookingDbContext bookingDbContext, IConnectionFactory connectionFactory)
    {
        _bookingDbContext = bookingDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserActivatedEvent> context)
    {
        var @event = context.Message;
        var user = await _bookingDbContext.BookingUsers
            .Where(x => x.UserId == @event.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);
        if (user != null)
        {
            user.IsActive = true;
            _bookingDbContext.BookingUsers.Update(user);

            await _bookingDbContext.SaveChangesAsync(context.CancellationToken);
            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserActivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserActivatedEventConsumer>("user-activated-booking-queue", "user-activated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserActivatedEventConsumer,
        UserActivatedEventConsumerDefinition>;
}
