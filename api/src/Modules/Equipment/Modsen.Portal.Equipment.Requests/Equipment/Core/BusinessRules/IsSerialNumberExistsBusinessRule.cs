using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.DataAccess.Npgsql;

namespace Modsen.Portal.Equipment.Requests.Equipment.Core.BusinessRules;

internal interface IIsSerialNumberExistsBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(string serialNumber, int? updatingEquipmentId, CancellationToken cancellationToken);
}

internal class IsSerialNumberExistsBusinessRule : IIsSerialNumberExistsBusinessRule
{
    private readonly IEquipmentDbContext _dbContext;

    public IsSerialNumberExistsBusinessRule(IEquipmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> IsCorrespondsToBusiness(string serialNumber, int? updatingEquipmentId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Equipments.Where(equipment => equipment.SerialNumber == serialNumber);

        if (updatingEquipmentId is not null)
        {
            query = query.Where(equipment => !(equipment.SerialNumber == serialNumber &&
                equipment.Id == updatingEquipmentId));
        }

        return query.AnyAsync(cancellationToken);
    }
}
