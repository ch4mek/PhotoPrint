using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Booking.Requests.Consumers;

internal sealed class UserDeactivatedEventConsumer : IConsumer<UserDeactivatedEvent>
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserDeactivatedEventConsumer(IBookingDbContext bookingDbContext, IConnectionFactory connectionFactory)
    {
        _bookingDbContext = bookingDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserDeactivatedEvent> context)
    {
        var @event = context.Message;
        var user = await _bookingDbContext.BookingUsers
            .Where(x => x.UserId == @event.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);
        if (user != null)
        {
            user.IsActive = false;
            _bookingDbContext.BookingUsers.Update(user);

            await _bookingDbContext.SaveChangesAsync(context.CancellationToken);
            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserDeactivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserDeactivatedEventConsumer>("user-deactivated-booking-queue", "user-deactivated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserDeactivatedEventConsumer,
        UserDeactivatedEventConsumerDefinition>;
}
