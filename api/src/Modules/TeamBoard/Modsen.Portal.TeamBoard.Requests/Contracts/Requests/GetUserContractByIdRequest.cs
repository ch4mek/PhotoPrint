using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetUserContractByIdRequest : IQueryHandler<GetUserContractByIdRequest.Query, UserContractDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IIsUserHasPermissionsToViewContractsBusinessRule _isUserHasPermissionsToViewContractsBusinessRule;

    public GetUserContractByIdRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IIsUserHasPermissionsToViewContractsBusinessRule isUserHasPermissionsToViewContractsBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _isUserHasPermissionsToViewContractsBusinessRule = isUserHasPermissionsToViewContractsBusinessRule;
    }

    public async Task<UserContractDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var userContract = await _teamBoardDbContext.UserContracts
            .Where(contract => contract.Id == request.Id)
            .Select(contract => new UserContractDto(
                contract.Id,
                contract.UserId,
                contract.ContractFrom,
                contract.ContractTo,
                contract.FireDate,
                contract.FireType,
                contract.FireReason,
                contract.FireDescription))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"There is no contract with {request.Id}");

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

        var businessRuleResult = await _isUserHasPermissionsToViewContractsBusinessRule.IsCorrespondsToBusiness
            (new IsUserHasPermissionBusinessModel(userContract.UserId, user.Position, user.DepartmentId,
                user.ResourceGroupId, user.IsActive),
            cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResult.Reason ?? string.Empty);
        }

        return userContract;
    }

    internal sealed record Query(int Id) : IQuery<UserContractDto>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
        }
    }
}
