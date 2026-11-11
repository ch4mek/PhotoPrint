using Modsen.Portal.Equipment.Requests.Dto;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public record EquipmentTypeOutput(string Id, string ShortName, string FullName)
{
    public static EquipmentTypeOutput? Of(EquipmentTypeDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new EquipmentTypeOutput(
            source.Id,
            source.ShortName,
            source.FullName);
    }
}
