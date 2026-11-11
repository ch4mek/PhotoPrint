using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal abstract class UserEventConsumerBase<TEvent> : IConsumer<TEvent> where TEvent : class
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly AttendanceDbContext _dbContext;

    protected UserEventConsumerBase(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var @event = context.Message;

        var userPosition = GetUserPosition(@event);

        if (userPosition.IsIntern())
        {
            return;
        }

        var userId = GetUserId(@event);

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.UserId == userId, context.CancellationToken);

        if (user is null)
        {
            user = CreateUser(@event);
            _dbContext.Users.Add(user);
        }
        else
        {
            UpdateUser(user, @event);
        }

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    protected abstract int GetUserId(TEvent @event);
    protected abstract string? GetUserPosition(TEvent @event);
    protected abstract UserEntity CreateUser(TEvent userSyncEvent);
    protected abstract void UpdateUser(UserEntity user, TEvent userSyncEvent);
}
