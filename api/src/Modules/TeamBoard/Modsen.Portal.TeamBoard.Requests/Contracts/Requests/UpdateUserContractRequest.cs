using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class UpdateUserContractRequest : ICommandHandler<UpdateUserContractRequest.Command, UpdateUserContractDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IIsFireDateLaterThanHireDateDbQuery _isFireDateLaterThanHireDateDbQuery;
    private readonly IIsUserHasPermissionsToMutateContractsBusinessRule _isUserHasPermissionsToMutateContractsBusinessRule;
    private readonly ICommunicationBus _communicationBus;

    public UpdateUserContractRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamBoardDbContext,
        IIsFireDateLaterThanHireDateDbQuery isFireDateLaterThanHireDateDbQuery,
        IIsUserHasPermissionsToMutateContractsBusinessRule isUserHasPermissionsToMutateContractsBusinessRule,
        ICommunicationBus communicationBus)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
        _isFireDateLaterThanHireDateDbQuery = isFireDateLaterThanHireDateDbQuery;
        _isUserHasPermissionsToMutateContractsBusinessRule = isUserHasPermissionsToMutateContractsBusinessRule;
        _communicationBus = communicationBus;
    }

    public async Task<UpdateUserContractDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var userContract = await _teamBoardDbContext.UserContracts
            .FirstOrDefaultAsync(contract => contract.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"There is no contract with Id {request.Id}");

        if (_authenticatedUserContext.GetRequiredUserId() == userContract.UserId)
        {
            throw new ActionNotAllowedException("You are not allowed to edit your own contract");
        }

        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == userContract.UserId)
            .Select(user => new
            {
                user.Position,
                user.ResourceGroupId,
                user.DepartmentId,
                user.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityConflictException($"There is no user with Id {userContract.UserId}");

        var businessRuleResult = await _isUserHasPermissionsToMutateContractsBusinessRule.IsCorrespondsToBusiness
            (new IsUserHasPermissionBusinessModel(userContract.UserId, user.Position, user.DepartmentId,
                user.ResourceGroupId, user.IsActive),
            cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResult.Reason ?? string.Empty);
        }

        if (request.FireDate != null &&
            !await _isFireDateLaterThanHireDateDbQuery
                .ExecuteAsync(userContract.UserId, request.FireDate.Value, cancellationToken))
        {
            throw new InvalidDataAppException("Fire date must be later than Hire date and Start date of the current contract, and earlier than End date of the current contract");
        }

        var isUserContractPeriodOverlapsWithExistingOne = await IsUserContractPeriodOverlaps(
            request.Id,
            userContract.UserId,
            request.ContractFrom,
            request.ContractTo,
            cancellationToken);

        if (isUserContractPeriodOverlapsWithExistingOne)
        {
            throw new ActionNotAllowedException("Contract for this period already exists. Please check and try again");
        }

        userContract.ContractFrom = request.ContractFrom;
        userContract.ContractTo = request.ContractTo;
        userContract.FireDate = request.FireDate;
        userContract.FireType = request.FireType;
        userContract.FireReason = request.FireReason;
        userContract.FireDescription = request.FireDescription;
        userContract.UpdatedById = _authenticatedUserContext.UserId;
        userContract.UpdatedAtUtc = DateTime.UtcNow;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var notification = new ContractUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ContractId = userContract.Id,
            UserId = userContract.UserId,
            ContractFrom = userContract.ContractFrom,
            ContractTo = userContract.ContractTo!.Value,
            FireDate = userContract.FireDate,
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return new UpdateUserContractDto(
            userContract.Id,
            userContract.UserId,
            userContract.ContractFrom,
            userContract.ContractTo!.Value,
            userContract.FireDate,
            userContract.FireType,
            userContract.FireReason,
            userContract.FireDescription);
    }

    private async Task<bool> IsUserContractPeriodOverlaps(int id, int userId, DateOnly contractFrom, DateOnly contractTo, CancellationToken cancellationToken)
    {
        var allUserContracts = await _teamBoardDbContext.UserContracts
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        if (allUserContracts.Count == 0)
        {
            return false;
        }

        var latestContract = allUserContracts
            .OrderByDescending(x => x.ContractFrom)
            .FirstOrDefault();

        var thisContract = allUserContracts.FirstOrDefault(x => x.Id == id);

        if (latestContract is not null && thisContract is not null && latestContract.Id != thisContract.Id &&
            (latestContract.ContractTo == null && contractFrom >= latestContract.ContractFrom))
        {
            return true;
        }

        var isOverlapped = allUserContracts.Where(x => x.Id != id).Any(x =>
            (contractFrom >= x.ContractFrom && contractFrom <= x.ContractTo) ||
            (contractFrom >= x.ContractFrom && contractTo <= x.ContractTo) ||
            (contractFrom <= x.ContractFrom && contractTo >= x.ContractFrom)
        );

        return isOverlapped;
    }

    internal sealed record Command(
        int Id,
        DateOnly ContractFrom,
        DateOnly ContractTo,
        DateOnly? FireDate,
        FireType? FireType,
        string? FireReason,
        string? FireDescription)
        : ICommand<UpdateUserContractDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x => x.ContractFrom).NotNull();
            RuleFor(x => x.ContractTo).NotNull().GreaterThanOrEqualTo(x => x.ContractFrom);
            When(x => !x.FireReason.IsNullOrEmpty(), () => RuleFor(x => x.FireReason).MinimumLength(2).MaximumLength(1000));
            When(x => !x.FireDescription.IsNullOrEmpty(), () => RuleFor(x => x.FireDescription).MinimumLength(2).MaximumLength(1000));            
            When(x => x.FireDate is not null, () => RuleFor(x => x.FireDate).GreaterThan(x => x.ContractFrom));
            When(x => x.FireDate is not null, () => RuleFor(x => x.FireDate).LessThanOrEqualTo(x => x.ContractTo));
        }
    }
}
