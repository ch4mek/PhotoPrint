using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Core.DbQueries;

namespace Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

internal sealed class SetReturnDateToAssignRequest
    : ICommandHandler<SetReturnDateToAssignRequest.Command, EquipmentHolderDto>
{
    private readonly IIsEquipmentExistByIdDbQuery _isEquipmentExistByIdDbQuery;
    private readonly IEquipmentDbContext _equipmentDbContext;

    private CancellationToken _cancellationToken;

    public SetReturnDateToAssignRequest(IIsEquipmentExistByIdDbQuery isEquipmentExistByIdDbQuery,
        IEquipmentDbContext equipmentDbContext)
    {
        _isEquipmentExistByIdDbQuery = isEquipmentExistByIdDbQuery;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<EquipmentHolderDto> Handle(Command request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var isEquipmentExists = await _isEquipmentExistByIdDbQuery.ExecuteAsync(request.EquipmentId,
            _cancellationToken);
        if (!isEquipmentExists)
        {
            throw new EntityNotFoundException(PortalMessages.EquipmentNotFound);
        }

        var lastAssign = await ValidateAssignDatesAndReturnLastAssign(request);

        await SetReturnDateToAssign(lastAssign.Id, request.ReturnDate);

        return await GetEquipmentHolderAsync(lastAssign.Id);
    }

    private async Task<EquipmentAssignEntity> ValidateAssignDatesAndReturnLastAssign(Command request)
    {
        if (request.ReturnDate.Date > DateTime.UtcNow.Date)
        {
            throw new InvalidDataAppException(PortalMessages.ReturnDateWasSetForTheFuture);
        }

        var assigns = await _equipmentDbContext.EquipmentAssigns
            .Where(assign => !assign.IsDeleted && assign.EquipmentId == request.EquipmentId)
            .ToListAsync(_cancellationToken);

        if (assigns.Count == 0)
        {
            throw new InvalidDataAppException(PortalMessages.EquipmentWasNeverAssigned);
        }

        if (assigns.Count(x => x.ReturnDate == null) > 1)
        {
            throw new InvalidDataAppException(PortalMessages.SeveralNullDatesForEquipment);
        }

        var lastAssign = assigns.OrderByDescending(assign => assign.IssueDate)
                                .ThenByDescending(assign => assign.Id)
                                .First();

        if (lastAssign.ReturnDate != null)
        {
            throw new EntityConflictException(PortalMessages.ExistingReturnDateForLastAssignment);
        }

        if (lastAssign.IssueDate > request.ReturnDate)
        {
            throw new InvalidDataAppException(PortalMessages.ReturnDateLowerThanIssueDate);
        }

        return lastAssign;
    }

    private async Task SetReturnDateToAssign(int assignId, DateTime returnDate)
    {
        var assign = await _equipmentDbContext.EquipmentAssigns
            .FirstAsync(assign => !assign.IsDeleted && assign.Id == assignId, _cancellationToken);

        assign.ReturnDate = returnDate;

        await _equipmentDbContext.SaveChangesAsync(_cancellationToken);
    }

    private Task<EquipmentHolderDto> GetEquipmentHolderAsync(int assignId)
    {
        return _equipmentDbContext.EquipmentAssigns
            .Include(assign => assign.User)
            .Include(assign => assign.Approver)
            .Where(assign => !assign.IsDeleted && assign.Id == assignId)
            .Select(assign => new EquipmentHolderDto
            {
                EquipmentHistoryId = assign.Id,
                Id = assign.User!.UserId,
                FirstName = assign.User.FirstName,
                LastName = assign.User.LastName,
                DepartmentId = assign.User.DepartmentId,
                IssueDate = assign.IssueDate,
                ReturnDate = assign.ReturnDate,
                ApproverId = assign.ApproverId,
                ApproverFirstName = assign.Approver == null ? null : assign.Approver.FirstName,
                ApproverLastName = assign.Approver == null ? null : assign.Approver.LastName
            })
            .FirstOrDefaultAsync(_cancellationToken)!;
    }

    internal sealed record Command(int EquipmentId, DateTime ReturnDate)
        : ICommand<EquipmentHolderDto>, IEquipmentRequest, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.EquipmentId)
                .GreaterThan(0);

            RuleFor(x => x.ReturnDate)
                .DateRule();
        }
    }
}


