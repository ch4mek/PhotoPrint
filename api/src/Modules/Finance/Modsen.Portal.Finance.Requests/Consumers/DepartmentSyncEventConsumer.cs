using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class DepartmentSyncEventConsumer : IConsumer<DepartmentSyncEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IFinanceDbContext _dbContext;

    public DepartmentSyncEventConsumer(IConnectionFactory connectionFactory, IFinanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<DepartmentSyncEvent> context)
    {
        var @event = context.Message;

        var department = await _dbContext.FinanceDepartments
            .FirstOrDefaultAsync(x => x.DepartmentId == @event.DepartmentId, context.CancellationToken);

        if (department == null)
        {
            department = new FinanceDepartmentEntity
            {
                DepartmentId = @event.DepartmentId,
                FullName = @event.DepartmentFullName,
                ShortName = @event.DepartmentShortName,
                DepartmentOrderWeight = @event.DepartmentOrderWeight
            };

            _dbContext.FinanceDepartments.Add(department);
        }
        else
        {
            department.FullName = @event.DepartmentFullName;
            department.ShortName = @event.DepartmentShortName;
            department.DepartmentOrderWeight = @event.DepartmentOrderWeight;
        }        

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class DepartmentSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<DepartmentSyncEventConsumer>(
            "department-sync-finance-queue",
            "department-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        DepartmentSyncEventConsumer,
        DepartmentSyncEventConsumerDefinition>;
}
