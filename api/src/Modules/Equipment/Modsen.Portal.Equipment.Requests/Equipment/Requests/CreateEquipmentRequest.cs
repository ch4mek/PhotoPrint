using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Generators;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests;

internal sealed class CreateEquipmentRequest : ICommandHandler<CreateEquipmentRequest.Command, EquipmentCoreDto>
{
    private readonly IEquipmentInventoryNumberGenerator _equipmentInventoryNumberGenerator;
    private readonly IIsEquipmentTypeExistsDbQuery _isEquipmentTypeExistsDbQuery;
    private readonly IIsSerialNumberExistsBusinessRule _isSerialNumberExistsBusinessRule;
    private readonly IEquipmentDbContext _equipmentDbContext;
    private readonly IIdentityContract _identityContract;

    public CreateEquipmentRequest(
        IEquipmentInventoryNumberGenerator equipmentInventoryNumberGenerator,
        IIsEquipmentTypeExistsDbQuery isEquipmentTypeExistsDbQuery,
        IIsSerialNumberExistsBusinessRule isSerialNumberExistsBusinessRule,
        IEquipmentDbContext equipmentDbContext,
        IIdentityContract identityContract)
    {
        _equipmentInventoryNumberGenerator = equipmentInventoryNumberGenerator;
        _isEquipmentTypeExistsDbQuery = isEquipmentTypeExistsDbQuery;
        _isSerialNumberExistsBusinessRule = isSerialNumberExistsBusinessRule;
        _equipmentDbContext = equipmentDbContext;
        _identityContract = identityContract;
    }

    public async Task<EquipmentCoreDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;
        await ValidateRequest(request, cancellationToken);

        var equipmentToCreate = request.ToDatabaseEntity();
        equipmentToCreate.InventoryNumber = await _equipmentInventoryNumberGenerator.Generate(equipmentToCreate.PurchaseDate, cancellationToken);

        await _equipmentDbContext.Equipments.AddAsync(equipmentToCreate, cancellationToken);
        await _equipmentDbContext.SaveChangesAsync(cancellationToken);

        var (type, approver) = await GetEquipmentTypeAndUserAsync(equipmentToCreate.Id, cancellationToken);

        equipmentToCreate.Approver = approver;
        equipmentToCreate.Type = type;

        return EquipmentCoreDto.Of(equipmentToCreate);
    }

    private async Task ValidateRequest(CreateEquipmentRequestDto request, CancellationToken cancellationToken)
    {
        var isUserExisting = await _equipmentDbContext.EquipmentUsers
            .AnyAsync(user => user.UserId == request.ApproverId, cancellationToken);

        if (!isUserExisting)
        {
            throw new EntityNotFoundException(PortalMessages.ApproverDoesNotExist);
        }

        var isApproverManager = await _identityContract.IsUserManager(request.ApproverId, cancellationToken);
        if (!isApproverManager)
        {
            throw new ActionNotAllowedException(PortalMessages.ApproverDoesNotHavePermission);
        }

        if (!await _isEquipmentTypeExistsDbQuery.ExecuteAsync(request.EquipmentTypeId, cancellationToken))
        {
            throw new EntityNotFoundException(PortalMessages.EquipmentTypeNotFound);
        }

        if (await _isSerialNumberExistsBusinessRule.IsCorrespondsToBusiness(request.SerialNumber, null, cancellationToken))
        {
            throw new InvalidDataAppException(PortalMessages.ExistingSerialNumber);
        }

        if (request is { BrokenStatus: EquipmentBrokenStatus.Renovation, RepairPlace: null })
        {
            throw new InvalidDataAppException(PortalMessages.MandatoryRepairPlace);
        }
    }

    private async Task<Tuple<EquipmentTypeEntity, EquipmentUser>> GetEquipmentTypeAndUserAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _equipmentDbContext.Equipments
            .Include(equipment => equipment.Approver)
            .Include(equipment => equipment.Type)
            .Where(equipment => equipment.Id == id)
            .Select(equipment => new
            {
                equipment.Approver,
                equipment.Type
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new Tuple<EquipmentTypeEntity, EquipmentUser>(result!.Type!, result.Approver!);
    }

    internal sealed record Command(
        CreateEquipmentRequestDto RequestDto) : ICommand<EquipmentCoreDto>, IEquipmentRequest, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new CreateEquipmentRequestDto.Validator());
        }
    }
}
