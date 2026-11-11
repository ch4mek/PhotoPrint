using Modsen.Portal.Equipment.Contract.Models;

namespace Modsen.Portal.Equipment.Contract;

public interface IEquipmentContract
{
    Task<bool> CanEquipmentUserBeDeactivated(int userId, CancellationToken cancellationToken);

    Task<List<EquipmentTypeClassificatorDto>> GetEquipmentTypes(CancellationToken cancellationToken);
}
