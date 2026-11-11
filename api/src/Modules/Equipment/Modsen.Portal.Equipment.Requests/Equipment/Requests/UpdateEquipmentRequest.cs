using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.Cache;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests;

internal sealed class UpdateEquipmentRequest : ICommandHandler<UpdateEquipmentRequest.Command, UpdatedEquipmentDto>
{
    private readonly IIsEquipmentTypeExistsDbQuery _isEquipmentTypeExistDbQuery;
    private readonly IIsSerialNumberExistsBusinessRule _isSerialNumberExistsBusinessRule;
    private readonly IIsInventoryNumberExistBusinessRule _isInventoryNumberExistBusinessRule;
    private readonly IGetEquipmentByIdDbQuery _getEquipmentByIdDbQuery;
    private readonly IEquipmentTypesCache _equipmentTypesCache;
    private readonly IEquipmentDbContext _equipmentDbContext;
    private readonly IIdentityContract _identityContract;

    public UpdateEquipmentRequest(
        IIsEquipmentTypeExistsDbQuery isEquipmentTypeExistDbQuery,
        IIsSerialNumberExistsBusinessRule isSerialNumberExistsBusinessRule,
        IIsInventoryNumberExistBusinessRule isInventoryNumberExistBusinessRule,
        IGetEquipmentByIdDbQuery getEquipmentByIdDbQuery,
        IEquipmentTypesCache equipmentTypesCache,
        IEquipmentDbContext equipmentDbContext,
        IIdentityContract identityContract)
    {
        _isEquipmentTypeExistDbQuery = isEquipmentTypeExistDbQuery;
        _isSerialNumberExistsBusinessRule = isSerialNumberExistsBusinessRule;
        _isInventoryNumberExistBusinessRule = isInventoryNumberExistBusinessRule;
        _getEquipmentByIdDbQuery = getEquipmentByIdDbQuery;
        _equipmentTypesCache = equipmentTypesCache;
        _equipmentDbContext = equipmentDbContext;
        _identityContract = identityContract;
    }

    public async Task<UpdatedEquipmentDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        var equipmentApproverId = await _equipmentDbContext.EquipmentUsers
            .Where(user => user.UserId == request.ApproverId && user.IsActive)
            .Select(user => user.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (equipmentApproverId == 0)
        {
            throw new EntityNotFoundException(PortalMessages.ApproverDoesNotExist);
        }

        await CheckIfApproverHasPermissionsAsync(equipmentApproverId, cancellationToken);

        await ValidateRequestAsync(request, cancellationToken);

        var updatedEquipment = await UpdateEquipment(request, equipmentApproverId, cancellationToken);

        return UpdatedEquipmentDto.Of(updatedEquipment);
    }

    private async Task<EquipmentEntity> UpdateEquipment(UpdateEquipmentRequestDto request,
        int approverId, CancellationToken cancellationToken)
    {
        var typeId = (await _equipmentTypesCache
            .GetEquipmentTypeById(request.EquipmentTypeId, cancellationToken))!.Id!;

        var equipment = await _equipmentDbContext.Equipments
            .Include(x => x.Type)
            .Include(x => x.Approver)
            .FirstAsync(x => x.Id == request.Id, cancellationToken);

        var equipmentType = await _equipmentDbContext.EquipmentTypes
            .FirstAsync(x => x.Id == typeId, cancellationToken);

        var equipmentApprover = await _equipmentDbContext.EquipmentUsers
            .FirstAsync(x => x.UserId == approverId, cancellationToken);

        equipment.Name = request.Name;
        equipment.Type = equipmentType;
        equipment.Location = request.Location;
        equipment.BrokenStatus = request.BrokenStatus;
        equipment.SerialNumber = request.SerialNumber;
        equipment.InventoryNumber = request.InventoryNumber;
        equipment.PurchasePrice = request.PurchasePrice;
        equipment.PurchaseCurrency = request.PurchaseCurrency;
        equipment.PurchasePriceUsd = request.PurchasePriceUsd;
        equipment.PurchaseDate = request.PurchaseDate;
        equipment.PurchasePlace = request.PurchasePlace;
        equipment.GuaranteeDate = request.GuaranteeDate;
        equipment.Characteristics = request.Characteristics;
        equipment.Comment = request.Comment;
        equipment.InvoiceNumber = request.InvoiceNumber;
        equipment.Storage = request.Storage;
        equipment.Approver = equipmentApprover;
        equipment.RepairPlace = request.RepairPlace;
        equipment.JiraTask = request.JiraTask;

        await _equipmentDbContext.SaveChangesAsync(cancellationToken);

        return equipment;
    }

    private async Task ValidateRequestAsync(UpdateEquipmentRequestDto request, CancellationToken cancellationToken)
    {
        if (request is { BrokenStatus: EquipmentBrokenStatus.Renovation, RepairPlace: null })
        {
            throw new InvalidDataAppException(PortalMessages.MandatoryRepairPlace);
        }

        if (!await _isEquipmentTypeExistDbQuery.ExecuteAsync(request.EquipmentTypeId, cancellationToken))
        {
            throw new EntityNotFoundException(PortalMessages.EquipmentTypeNotFound);
        }

        if (await _isInventoryNumberExistBusinessRule.IsCorrespondsToBusiness(request.InventoryNumber, request.Id, cancellationToken))
        {
            throw new InvalidDataAppException(PortalMessages.ExistingInventoryNumber);
        }

        var equipment = await _getEquipmentByIdDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.EquipmentNotFound);

        var activeAssign = equipment.Assigns.Where(assign => assign is { IsDeleted: false, ReturnDate: null })
            .MinBy(equipmentAssign => equipmentAssign.IssueDate);

        if (activeAssign != null && request.BrokenStatus == EquipmentBrokenStatus.Writtenoff)
        {
            throw new InvalidDataAppException(PortalMessages.BrokenStatusConflict);
        }

        if (await _isSerialNumberExistsBusinessRule.IsCorrespondsToBusiness(request.SerialNumber, request.Id, cancellationToken))
        {
            throw new InvalidDataAppException(PortalMessages.ExistingSerialNumber);
        }
    }

    private async Task CheckIfApproverHasPermissionsAsync(
        int approverId,
        CancellationToken cancellationToken)
    {
        var isApproverManager = await _identityContract.IsUserManager(
            approverId,
            cancellationToken);

        if (!isApproverManager)
        {
            throw new ActionNotAllowedException(PortalMessages.ApproverDoesNotHavePermission);
        }
    }

    internal sealed record Command(UpdateEquipmentRequestDto RequestDto)
        : ICommand<UpdatedEquipmentDto>, IEquipmentRequest, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new UpdateEquipmentRequestDto.Validator());
        }
    }
}

