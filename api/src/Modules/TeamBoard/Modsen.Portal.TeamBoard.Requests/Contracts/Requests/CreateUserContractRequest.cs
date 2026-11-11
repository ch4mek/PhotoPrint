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
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class CreateUserContractRequest : ICommandHandler<CreateUserContractRequest.Command, CreateUserContractDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIsFireDateLaterThanHireDateDbQuery _isFireDateLaterThanHireDateDbQuery;
    private readonly IIsUserHasPermissionsToMutateContractsBusinessRule _isUserHasPermissionsToMutateContractsBusinessRule;
    private readonly ICommunicationBus _communicationBus;

    public CreateUserContractRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IIsFireDateLaterThanHireDateDbQuery isFireDateLaterThanHireDateDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        IIsUserHasPermissionsToMutateContractsBusinessRule isUserHasPermissionsToMutateContractsBusinessRule,
        ICommunicationBus communicationBus)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _isFireDateLaterThanHireDateDbQuery = isFireDateLaterThanHireDateDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _isUserHasPermissionsToMutateContractsBusinessRule = isUserHasPermissionsToMutateContractsBusinessRule;
        _communicationBus = communicationBus;
    }

    public async Task<CreateUserContractDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == request.UserId)
            .Select(user => new
            {
                user.Position,
                user.ResourceGroupId,
                user.DepartmentId,
                user.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"There is no user with Id {request.UserId}");

        var businessRuleResult = await _isUserHasPermissionsToMutateContractsBusinessRule.IsCorrespondsToBusiness
            (new IsUserHasPermissionBusinessModel(request.UserId, user.Position, user.DepartmentId,
                user.ResourceGroupId, user.IsActive),
            cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResult.Reason ?? string.Empty);
        }

        if (request.FireDate is not null &&
            !await _isFireDateLaterThanHireDateDbQuery.ExecuteAsync(
                request.UserId, request.FireDate.Value, cancellationToken))
        {
            throw new InvalidDataAppException("Fire date must be later than Hire date and Start date of the current contract, and earlier than End date of the current contract");
        }

        var isUserContractPeriodOverlapsWithExistingOne = await IsContractPeriodOverlaps(request.UserId, request.ContractFrom, request.ContractTo);

        if (isUserContractPeriodOverlapsWithExistingOne)
        {
            throw new ActionNotAllowedException("Contract for this period already exists. Please check and try again");
        }

        var response = await _teamBoardDbContext.UserContracts.AddAsync(new UserContract
        {
            UserId = request.UserId,
            ContractFrom = request.ContractFrom,
            ContractTo = request.ContractTo,
            FireDate = request.FireDate,
            FireType = request.FireType,
            FireReason = request.FireReason,
            FireDescription = request.FireDescription,
            IsActive = true,
            CreatedById = _authenticatedUserContext.UserId,
            CreatedAtUtc = DateTime.UtcNow,
            IsProbationPeriod = false
        }, cancellationToken);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var notification = new ContractCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ContractId = response.Entity.Id,
            UserId = request.UserId,
            ContractFrom = request.ContractFrom,
            ContractTo = request.ContractTo,
            FireDate = request.FireDate,
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return new CreateUserContractDto(
            response.Entity.Id,
            response.Entity.UserId,
            response.Entity.ContractFrom,
            response.Entity.ContractTo!.Value,
            response.Entity.FireDate,
            response.Entity.FireType,
            response.Entity.FireReason,
            response.Entity.FireDescription);
    }

    private async Task<bool> IsContractPeriodOverlaps(int userId, DateOnly contractFrom, DateOnly contractTo)
    {
        var allUserContracts = await _teamBoardDbContext.UserContracts
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                x.ContractFrom,
                x.ContractTo
            })
            .ToListAsync();

        var latestContract = allUserContracts
            .OrderByDescending(x => x.ContractFrom)
            .FirstOrDefault();

        if (latestContract != null && (latestContract.ContractTo == null && contractFrom >= latestContract.ContractFrom))
        {
            return true;
        }

        var isOverlapped = allUserContracts.Any(x =>
            (contractFrom >= x.ContractFrom && contractFrom <= x.ContractTo) ||
            (contractFrom >= x.ContractFrom && contractTo <= x.ContractTo) ||
            (contractFrom <= x.ContractFrom && contractTo >= x.ContractFrom)
        );

        return isOverlapped;
    }

    internal sealed record Command(
        int UserId,
        DateOnly ContractFrom,
        DateOnly ContractTo,
        DateOnly? FireDate,
        FireType? FireType,
        string? FireReason,
        string? FireDescription) : ICommand<CreateUserContractDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotNull().NotEmpty();
            RuleFor(x => x.ContractFrom).NotNull();
            RuleFor(x => x.ContractTo).NotNull().GreaterThanOrEqualTo(x => x.ContractFrom);
            When(x => !x.FireReason.IsNullOrEmpty(), () => RuleFor(x => x.FireReason).MinimumLength(2).MaximumLength(1000));
            When(x => !x.FireDescription.IsNullOrEmpty(), () => RuleFor(x => x.FireDescription).MinimumLength(2).MaximumLength(1000));
            When(x => x.FireDate is not null, () => RuleFor(x => x.FireDate).GreaterThan(x => x.ContractFrom));
            When(x => x.FireDate is not null, () => RuleFor(x => x.FireDate).LessThanOrEqualTo(x => x.ContractTo));
        }
    }
}
