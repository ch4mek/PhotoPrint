using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Equipment.Requests.Consumers;

internal sealed class DepartmentSyncEventConsumer : IConsumer<DepartmentSyncEvent>
{
    private readonly IEquipmentDbContext _equipmentDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public DepartmentSyncEventConsumer(IConnectionFactory connectionFactory, IEquipmentDbContext equipmentDbContext)
    {
        _connectionFactory = connectionFactory;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task Consume(ConsumeContext<DepartmentSyncEvent> context)
    {
        var @event = context.Message;

        await SyncDepartmentsAsync(
            @event.DepartmentId,
            @event.DepartmentFullName,
            @event.DepartmentShortName,
            context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private async Task SyncDepartmentsAsync(string departmentId, string fullName, string shortName,
        CancellationToken cancellationToken)
    {
        var department = await _equipmentDbContext.EquipmentDepartments
            .FirstOrDefaultAsync(x => x.DepartmentId == departmentId, cancellationToken);
        if (department == null)
        {
            department = new EquipmentDepartmentEntity
            {
                DepartmentId = departmentId,
                FullName = fullName,
                ShortName = shortName
            };

            _equipmentDbContext.EquipmentDepartments.Add(department);
        }
        else
        {
            department.FullName = fullName;
            department.ShortName = shortName;
            _equipmentDbContext.EquipmentDepartments.Update(department);
        }

        await _equipmentDbContext.SaveChangesAsync(cancellationToken);
    }

    internal sealed class DepartmentSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<DepartmentSyncEventConsumer>("department-sync-equipment-queue", "department-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        DepartmentSyncEventConsumer,
        DepartmentSyncEventConsumerDefinition>;
}
