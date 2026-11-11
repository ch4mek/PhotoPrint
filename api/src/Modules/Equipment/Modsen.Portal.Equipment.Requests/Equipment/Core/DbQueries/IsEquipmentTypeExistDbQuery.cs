using Modsen.Portal.Equipment.DataAccess.Npgsql.Cache;

namespace Modsen.Portal.Equipment.Requests.Equipment.Core.DbQueries;

internal interface IIsEquipmentTypeExistsDbQuery
{
    Task<bool> ExecuteAsync(string equipmentTypeId, CancellationToken cancellationToken);
}

internal class IsEquipmentTypeExistsDbQuery : IIsEquipmentTypeExistsDbQuery
{
    private readonly IEquipmentTypesCache _equipmentTypesCache;

    public IsEquipmentTypeExistsDbQuery(IEquipmentTypesCache equipmentTypesCache)
    {
        _equipmentTypesCache = equipmentTypesCache;
    }

    public async Task<bool> ExecuteAsync(string equipmentTypeId, CancellationToken cancellationToken)
    {
        var equipmentTypes = await _equipmentTypesCache.GetAllEquipmentTypes(cancellationToken);

        return equipmentTypes.Any(et => et.Id == equipmentTypeId);
    }
}
