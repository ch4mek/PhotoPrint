using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.Seeds;

internal class EquipmentTypeSeed : ISeedEntitiesProvider<EquipmentDbContext>
{
    private readonly EquipmentDbContext _dbContext;
    private readonly IClockService _clock;

    public EquipmentTypeSeed(EquipmentDbContext dbContext, IClockService clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public void Seed()
    {
        foreach (var equipmentType in GetEquipmentTypes())
        {
            CreateOrUpdate(equipmentType);
        }

        _dbContext.SaveChanges();
    }

    private void CreateOrUpdate(EquipmentTypeEntity equipmentTypeEntity)
    {
        var existingEquipmentType = _dbContext
            .Set<EquipmentTypeEntity>()
            .FirstOrDefault(x => x.Id == equipmentTypeEntity.Id);

        if (existingEquipmentType is null)
        {
            _dbContext.Set<EquipmentTypeEntity>().Add(equipmentTypeEntity);
        }
        else
        {
            existingEquipmentType.ShortName = equipmentTypeEntity.ShortName;
            existingEquipmentType.FullName = equipmentTypeEntity.FullName;
            existingEquipmentType.OrderWeight = equipmentTypeEntity.OrderWeight;
        }
    }

    private IEnumerable<EquipmentTypeEntity> GetEquipmentTypes()
    {
        var weight = 0;

        yield return Create("DesktopComputer", "DesktopComputer", "Desktop computer", weight += 100);
        yield return Create("Laptop", "Laptop", "Laptop", weight += 100);
        yield return Create("MacBook", "MacBook", "MacBook", weight += 100);
        yield return Create("MacBookMini", "MacMini", "Mac mini", weight += 100);
        yield return Create("Tablet", "Tablet", "Tablet", weight += 100);
        yield return Create("MobilePhone", "MobilePhone", "Mobile phone", weight += 100);
        yield return Create("Monitor", "Monitor", "Monitor", weight += 100);
        yield return Create("Keyboard", "Keyboard", "Keyboard", weight += 100);
        yield return Create("Mouse", "Mouse", "Mouse", weight += 100);
        yield return Create("MouseWithKeyboard", "MouseWithKeyboard", "Mouse + keyboard", weight += 100);
        yield return Create("Headset", "Headset", "Headset", weight += 100);
        yield return Create("WebCamera", "WebCamera", "Web camera", weight += 100);
        yield return Create("Firewall", "Firewall", "Firewall", weight += 100);
        yield return Create("AccessPoint", "AccessPoint", "Access point", weight += 100);
        yield return Create("Switch", "Switch", "Switch", weight += 100);
        yield return Create("Router", "Router", "Router", weight += 100);
        yield return Create("NetworkAdapter", "NetworkAdapter", "Network adapter", weight += 100);
        yield return Create("Printer", "Printer", "Printer", weight += 100);
        yield return Create("FlashDrive", "FlashDrive", "Flash drive", weight += 100);
        yield return Create("ExternalHardDrive", "ExternalHardDrive", "External hard drive", weight += 100);
        yield return Create("MultiAdapter", "MultiAdapter", "Multi-adapter", weight += 100);
        yield return Create("PcAccessoriesHdd", "PcAccessoriesHdd", "PC accessory: HDD", weight += 100);
        yield return Create("PcAccessoriesSsd", "PcAccessoriesSsd", "PC accessory: SSD", weight += 100);
        yield return Create("PcAccessoriesRam", "PcAccessoriesRam", "PC accessory: RAM", weight += 100);
        yield return Create("Other", "Other", "Other", weight += 100);

        EquipmentTypeEntity Create(string id, string shortName, string fullname, int orderWeight = 0)
        {
            return new EquipmentTypeEntity
            {
                Id = id,
                ShortName = shortName,
                FullName = fullname,
                OrderWeight = orderWeight,
                CreatedAtUtc = _clock.UtcNow,
                CreatedById = null
            };
        }
    }
}
