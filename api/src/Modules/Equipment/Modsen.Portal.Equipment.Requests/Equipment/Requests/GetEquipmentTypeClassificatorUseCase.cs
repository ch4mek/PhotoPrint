using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.Contract.Models;
using Modsen.Portal.Equipment.DataAccess.Npgsql.Cache;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests;

internal sealed class GetEquipmentTypeClassificatorUseCase : IQueryHandler<GetEquipmentTypeClassificatorUseCase.Query,
    List<EquipmentTypeClassificatorDto>>
{
    private readonly IEquipmentTypesCache _equipmentTypesCache;

    public GetEquipmentTypeClassificatorUseCase(IEquipmentTypesCache equipmentTypesCache)
    {
        _equipmentTypesCache = equipmentTypesCache;
    }

    public async Task<List<EquipmentTypeClassificatorDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var equipmentTypes = await _equipmentTypesCache.GetAllEquipmentTypes(cancellationToken);

        return equipmentTypes
            .Select(equipmentType => new EquipmentTypeClassificatorDto(
                equipmentType.Id!, equipmentType.ShortName!, equipmentType.FullName!))
            .ToList();
    }

    internal sealed record Query : IQuery<List<EquipmentTypeClassificatorDto>>, IEquipmentRequest;
}
