using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.DataAccess.Npgsql;

namespace Modsen.Portal.Equipment.Requests.EquipmentHistory.Core.DbQueries;

internal interface IIsEquipmentExistByIdDbQuery
{
    Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal class IsEquipmentExistByIdDbQuery : IIsEquipmentExistByIdDbQuery
{
    private readonly IEquipmentDbContext _context;

    public IsEquipmentExistByIdDbQuery(IEquipmentDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _context.Equipments
            .AnyAsync(equipment => equipment.Id == id, cancellationToken);
    }
}
