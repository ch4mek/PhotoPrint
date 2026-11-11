using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Dto;

namespace Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

internal sealed class DeleteEquipmentHistoryRequest : ICommandHandler<DeleteEquipmentHistoryRequest.Command, EquipmentHolderDto>
{
    private readonly IGetEquipmentByIdDbQuery _getEquipmentByIdDbQuery;
    private readonly IEquipmentDbContext _equipmentDbContext;

    private CancellationToken _cancellationToken;

    public DeleteEquipmentHistoryRequest(IGetEquipmentByIdDbQuery getEquipmentByIdDbQuery,
        IEquipmentDbContext equipmentDbContext)
    {
        _getEquipmentByIdDbQuery = getEquipmentByIdDbQuery;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<EquipmentHolderDto> Handle(Command request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        var equipment = await _getEquipmentByIdDbQuery.ExecuteAsync(request.EquipmentId, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.EquipmentNotFound);

        if (equipment.Assigns.Count == 0)
        {
            throw new InvalidDataAppException(PortalMessages.EquipmentDoesNotHaveAssigns);
        }

        var canBeDeleted = await CanEquipmentHistoryBeDeletedAsync(request.EquipmentId, request.HistoryId);

        if (!canBeDeleted)
        {
            throw new InvalidDataAppException(PortalMessages.EquipmentCannotBeDeleted);
        }

        var currentAssignToDelete = equipment.Assigns.FirstOrDefault(assign => assign.Id == request.HistoryId &&
            !assign.IsDeleted);

        var deletedCurrentAssign = await DeleteEquipmentAssignAsync(currentAssignToDelete!.Id);

        return EquipmentHolderDto.Of(deletedCurrentAssign);
    }

    private async Task<bool> CanEquipmentHistoryBeDeletedAsync(int equipmentId, int historyId)
    {
        var odpEquipmentStatus = await _equipmentDbContext.Equipments
            .Include(e => e.Assigns)
            .Where(oe => oe.Id == equipmentId)
            .Select(OdpEquipment.Map)
            .Select(oe => oe.Status)
            .FirstOrDefaultAsync(_cancellationToken);

        var assignReturnDate = await _equipmentDbContext.EquipmentAssigns
            .Where(ea => ea.Id == historyId && !ea.IsDeleted)
            .Select(ea => ea.ReturnDate)
            .FirstOrDefaultAsync(_cancellationToken);

        return odpEquipmentStatus == "Issued" && assignReturnDate == null;
    }

    private async Task<EquipmentAssignEntity> DeleteEquipmentAssignAsync(int assignId)
    {
        var equipmentAssign = await _equipmentDbContext.EquipmentAssigns
            .Include(assign => assign.User)
            .Include(assign => assign.Approver)
            .FirstOrDefaultAsync(assign => !assign.IsDeleted && assign.Id == assignId,
                _cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.EquipmentAssignNotFound);

        equipmentAssign.IsDeleted = true;

        await _equipmentDbContext.SaveChangesAsync(_cancellationToken);

        return equipmentAssign;
    }

    internal sealed record Command(int EquipmentId, int HistoryId)
        : ICommand<EquipmentHolderDto>, IEquipmentRequest, ITransactionalRequest;
}
