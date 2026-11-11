using MediatR;
using Modsen.Portal.Equipment.Contract;
using Modsen.Portal.Equipment.Contract.Models;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;

namespace Modsen.Portal.Equipment.Requests;

internal class EquipmentContract : IEquipmentContract
{
    private readonly ICanEquipmentUserBeDeactivatedBusinessRule _canEquipmentUserBeDeactivatedBusinessRule;
    private readonly IMediator _mediator;

    public EquipmentContract(ICanEquipmentUserBeDeactivatedBusinessRule canEquipmentUserBeDeactivatedBusinessRule,
        IMediator mediator)
    {
        _canEquipmentUserBeDeactivatedBusinessRule = canEquipmentUserBeDeactivatedBusinessRule;
        _mediator = mediator;
    }

    public Task<bool> CanEquipmentUserBeDeactivated(int userId, CancellationToken cancellationToken)
    {
        return _canEquipmentUserBeDeactivatedBusinessRule.IsCorrespondsToBusiness(userId, cancellationToken);
    }

    public Task<List<EquipmentTypeClassificatorDto>> GetEquipmentTypes(CancellationToken cancellationToken)
    {
        return _mediator.Send(new GetEquipmentTypeClassificatorUseCase.Query(), cancellationToken);
    }
}
