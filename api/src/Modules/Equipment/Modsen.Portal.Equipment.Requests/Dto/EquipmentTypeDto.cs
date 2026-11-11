using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.Requests.Dto;

public record EquipmentTypeDto(string Id, string ShortName, string FullName)
{
    internal static EquipmentTypeDto? Of(EquipmentTypeEntity? source)
    {
        return source == null
            ? null
            : new EquipmentTypeDto(
                source.Id,
                source.ShortName,
                source.FullName);
    }
}
