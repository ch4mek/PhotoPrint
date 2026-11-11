using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal abstract class ResourceGroupEventConsumerBase<TEvent> : IConsumer<TEvent> where TEvent : class
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly AttendanceDbContext _dbContext;

    protected ResourceGroupEventConsumerBase(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var @event = context.Message;
        var resourceGroupId = GetResourceGroupId(@event);

        var group = await _dbContext.ResourceGroups
            .FirstOrDefaultAsync(g => g.ResourceGroupId == resourceGroupId, context.CancellationToken);

        if (group is null)
        {
            group = CreateResourceGroup(@event);
            _dbContext.ResourceGroups.Add(group);
        }
        else
        {
            UpdateResourceGroup(group, @event);
        }

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    protected abstract int GetResourceGroupId(TEvent @event);

    protected abstract ResourceGroupEntity CreateResourceGroup(TEvent @event);

    protected abstract void UpdateResourceGroup(ResourceGroupEntity group, TEvent @event);
}
