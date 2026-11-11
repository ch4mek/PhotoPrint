using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Core.DbQueries;

namespace Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

internal sealed class AssignUserToEquipmentRequest : ICommandHandler<AssignUserToEquipmentRequest.Command, EquipmentHolderDto>
{
    private readonly IIsEquipmentExistByIdDbQuery _isEquipmentExistDbQuery;
    private readonly IEquipmentDbContext _equipmentDbContext;

    private CancellationToken _cancellationToken;

    public AssignUserToEquipmentRequest(IIsEquipmentExistByIdDbQuery isEquipmentExistDbQuery, IEquipmentDbContext equipmentDbContext)
    {
        _isEquipmentExistDbQuery = isEquipmentExistDbQuery;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<EquipmentHolderDto> Handle(Command request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        var isEquipmentExist = await _isEquipmentExistDbQuery.ExecuteAsync(request.EquipmentId, cancellationToken);
        if (!isEquipmentExist)
        {
            throw new EntityNotFoundException(PortalMessages.EquipmentNotFound);
        }

        var user = await GetEquipmentUserById(request.UserId)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        var approver = await GetEquipmentUserById(request.ApproverId)
            ?? throw new EntityNotFoundException(PortalMessages.ApproverDoesNotExist);

        ValidateApprover(approver: approver, user: user);
        await ValidateAssignDatesAsync(request, request.EquipmentId);

        return await CreateAssignAsync(request, user, approver);
    }

    private Task<EquipmentUser?> GetEquipmentUserById(int userId)
    {
        return _equipmentDbContext.EquipmentUsers
            .AsNoTracking()
            .Where(user => user.IsActive && user.UserId == userId)
            .FirstOrDefaultAsync(_cancellationToken);
    }

    private static void ValidateApprover(EquipmentUser approver, EquipmentUser user)
    {
        var approverPosition = approver.Position!;

        var isApproverHeadFromAnotherDepartment = approver.DepartmentId != user.DepartmentId ||
            approver.DepartmentId is null;

        var isApproverGmFromAnotherGroup = approver.ResourceGroupId != user.ResourceGroupId ||
            approver.ResourceGroupId is null;

        var approverIsChiefOrSysAdmin = approverPosition.IsSystemAdministrator() ||
            approverPosition.IsCeo() ||
            approverPosition.IsCto();

        if (approverPosition.IsHeadOfDepartment())
        {
            if (isApproverHeadFromAnotherDepartment)
            {
                throw new ActionNotAllowedException(PortalMessages.ApproverDoesNotHavePermission);
            }
        }
        else if (approverPosition.IsGroupManager())
        {
            if (isApproverGmFromAnotherGroup)
            {
                throw new ActionNotAllowedException(PortalMessages.ApproverDoesNotHavePermission);
            }
        }
        else
        {
            if (!approverIsChiefOrSysAdmin)
            {
                throw new ActionNotAllowedException(PortalMessages.ApproverDoesNotHavePermission);
            }
        }
    }

    private async Task ValidateAssignDatesAsync(Command request, int equipmentId)
    {
        var equipmentAssignsReturnDates = await _equipmentDbContext.EquipmentAssigns
            .Where(assign => !assign.IsDeleted && assign.EquipmentId == equipmentId)
            .Select(assign => assign.ReturnDate)
            .ToArrayAsync(_cancellationToken);

        if (equipmentAssignsReturnDates.Length != 0)
        {
            if (equipmentAssignsReturnDates.Any(x => x is null))
            {
                throw new EntityConflictException(PortalMessages.EquipmentAlreadyAssigned);
            }

            var maxReturnDate = equipmentAssignsReturnDates.Max()!.Value;
            if (maxReturnDate.Date > request.IssueDate.Date)
            {
                throw new EntityConflictException(PortalMessages.EquipmentAlreadyAssigned);
            }
        }
    }

    private async Task<EquipmentHolderDto> CreateAssignAsync(Command request, EquipmentUser user,
        EquipmentUser approver)
    {
        var assignToCreate = new EquipmentAssignEntity
        {
            EquipmentId = request.EquipmentId,
            IssueDate = request.IssueDate,
            AssignedToUserId = user.UserId,
            ApproverId = approver.UserId
        };

        await _equipmentDbContext.EquipmentAssigns.AddAsync(assignToCreate, _cancellationToken);
        await _equipmentDbContext.SaveChangesAsync(_cancellationToken);

        return new EquipmentHolderDto
        {
            EquipmentHistoryId = assignToCreate.Id,
            Id = assignToCreate.AssignedToUserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DepartmentId = user.DepartmentId,
            ResourceGroupId = approver.ResourceGroupId,
            IssueDate = assignToCreate.IssueDate,
            ReturnDate = null,
            ApproverId = approver.UserId,
            ApproverFirstName = approver.FirstName,
            ApproverLastName = approver.LastName
        };
    }

    internal sealed record Command(
        int EquipmentId,
        int UserId,
        int ApproverId,
        DateTime IssueDate) : ICommand<EquipmentHolderDto>, IEquipmentRequest, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.EquipmentId)
                .GreaterThan(0);

            RuleFor(x => x.IssueDate)
                .DateRule();
        }
    }
}
