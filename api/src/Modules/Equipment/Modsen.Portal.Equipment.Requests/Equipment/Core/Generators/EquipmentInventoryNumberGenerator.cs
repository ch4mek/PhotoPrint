using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.DataAccess.Npgsql;

namespace Modsen.Portal.Equipment.Requests.Equipment.Core.Generators;

internal interface IEquipmentInventoryNumberGenerator
{
    Task<string> Generate(DateTime purchaseDate, CancellationToken cancellationToken);
}

internal class EquipmentInventoryNumberGenerator : IEquipmentInventoryNumberGenerator
{
    private readonly IEquipmentDbContext _equipmentDbContext;

    public EquipmentInventoryNumberGenerator(IEquipmentDbContext equipmentDbContext)
    {
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<string> Generate(DateTime purchaseDate, CancellationToken cancellationToken)
    {
        var formatedDate = purchaseDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        var inventoryNumbers = await _equipmentDbContext.Equipments
            .Where(x =>
                x.PurchaseDate == purchaseDate &&
                x.InventoryNumber != null)
            .Select(x => x.InventoryNumber)
            .ToListAsync(cancellationToken);

        var maxDayCounterNumber = 0;

        if (inventoryNumbers.Count > 0)
        {
            maxDayCounterNumber = inventoryNumbers
                .Select(x =>
                {
                    if (x is null || x.Length < 10)
                    {
                        return 0;
                    }

                    var startIndex = 8;
                    var endIndex = x.Length - 1;
                    var strNumber = x.Substring(startIndex, endIndex - startIndex);

                    return int.TryParse(strNumber, out var number) ? number : 0;
                })
                .Max();
        }

        return $"{formatedDate}{maxDayCounterNumber + 1}D";
    }
}
