using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.Requests.Core.Builders;

namespace Modsen.Portal.Equipment.Requests.Core.DbQueries;

internal interface IGetEquipmentByIdDbQuery
{
    Task<DataAccess.Npgsql.DatabaseEntities.EquipmentEntity?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal class GetEquipmentByIdDbQuery : IGetEquipmentByIdDbQuery
{
    private readonly IEquipmentQueryBuilder _equipmentQueryBuilder;

    public GetEquipmentByIdDbQuery(IEquipmentQueryBuilder equipmentQueryBuilder)
    {
        _equipmentQueryBuilder = equipmentQueryBuilder;
    }

    public Task<DataAccess.Npgsql.DatabaseEntities.EquipmentEntity?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _equipmentQueryBuilder.Build()
            .FirstOrDefaultAsync(equipment => equipment.Id == id, cancellationToken);
    }
}
