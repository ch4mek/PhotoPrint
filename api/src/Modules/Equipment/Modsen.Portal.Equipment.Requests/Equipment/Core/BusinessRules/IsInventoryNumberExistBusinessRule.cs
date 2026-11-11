using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.DataAccess.Npgsql;

namespace Modsen.Portal.Equipment.Requests.Equipment.Core.BusinessRules;

internal interface IIsInventoryNumberExistBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(string inventoryNumber, int updatingEquipmentId, CancellationToken cancellationToken);
}

internal class IsInventoryNumberExistBusinessRule : IIsInventoryNumberExistBusinessRule
{
    private readonly IEquipmentDbContext _dbContext;

    public IsInventoryNumberExistBusinessRule(IEquipmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> IsCorrespondsToBusiness(string inventoryNumber, int updatingEquipmentId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Equipments
            .Where(equipment => equipment.InventoryNumber == inventoryNumber)
            .Where(equipment => equipment.Id != updatingEquipmentId);

        return query.AnyAsync(cancellationToken);
    }
}
