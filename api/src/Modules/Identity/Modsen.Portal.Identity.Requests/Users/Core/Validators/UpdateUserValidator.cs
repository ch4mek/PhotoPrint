using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Core.Validators
{
    internal interface IUpdateUserValidator
    {
        Task Validate(UserDto user, UpdateUserGeneralInfoRequest.InternalCommand request, CancellationToken cancellationToken);
        Task Validate(UserDto user, UpdateUserRequest.Command request, CancellationToken cancellationToken);
    }

    internal class UpdateUserValidator : IUpdateUserValidator
    {
        private readonly ICanRoleHaveDepartmentBusinessRule _canRoleHaveDepartmentBusinessRule;
        private readonly IAuthenticatedUserContext _authenticatedUserContext;
        private readonly ICanUserWithResourceGroupBeCreatedBusinessRule _canUserWithResourceGroupBeCreatedBusinessRule;
        private readonly ICanRoleBeOccupiedBusinessRule _canRoleBeOccupiedBusinessRule;
        private readonly IIdentityDbContext _identityDbContext;
        private readonly IIsManagerUserUpdateValidBusinessRule _isManagerUserUpdateValidBusinessRule;
        private readonly ITeamBoardContract _teamBoardContract;

        public UpdateUserValidator(
            ICanRoleHaveDepartmentBusinessRule canRoleHaveDepartmentBusinessRule,
            IAuthenticatedUserContext authenticatedUserContext,
            ICanUserWithResourceGroupBeCreatedBusinessRule canUserWithResourceGroupBeCreatedBusinessRule,
            ICanRoleBeOccupiedBusinessRule canRoleBeOccupiedBusinessRule,
            IIdentityDbContext identityDbContext,
            IIsManagerUserUpdateValidBusinessRule isManagerUserUpdateValidBusinessRule,
            ITeamBoardContract teamBoardContract)
        {
            _canRoleHaveDepartmentBusinessRule = canRoleHaveDepartmentBusinessRule;
            _authenticatedUserContext = authenticatedUserContext;
            _canUserWithResourceGroupBeCreatedBusinessRule = canUserWithResourceGroupBeCreatedBusinessRule;
            _canRoleBeOccupiedBusinessRule = canRoleBeOccupiedBusinessRule;
            _identityDbContext = identityDbContext;
            _isManagerUserUpdateValidBusinessRule = isManagerUserUpdateValidBusinessRule;
            _teamBoardContract = teamBoardContract;
        }

        public async Task Validate(UserDto user, UpdateUserGeneralInfoRequest.InternalCommand request, CancellationToken cancellationToken)
        {
            CheckAllowedPosition(user, request.Position);
            await ValidateRequestEmail(request.Email, user.Email, cancellationToken);
            await ValidateDepartmentId(request.Position, request.DepartmentId, cancellationToken);
            await ValidateResourceGroupId(
                request.Position, request.ResourceGroupId, request.DepartmentId,
                user.Position, user.ResourceGroupId, user.DepartmentId,
                cancellationToken);
            await CheckIfRoleIsOccupied(user.Position, user.ResourceGroupId, request.ResourceGroupId, request.DepartmentId, cancellationToken);
            await ValidateInternPositionUpdate(user, request.Position, cancellationToken);

            var result = await _isManagerUserUpdateValidBusinessRule
                .IsCorrespondsToBusiness(new IsManagerUserUpdateValidBusinessRule.Request(
                    user.Position!,
                    request.DepartmentId,
                    request.ResourceGroupId,
                    user.Position!,
                    user.DepartmentId),
                    cancellationToken);

            if (!result.IsSuccess)
            {
                throw new ActionNotAllowedException(result.Reason!);
            }
        }

        public async Task Validate(UserDto user, UpdateUserRequest.Command request, CancellationToken cancellationToken)
        {
            await ValidateRequestEmail(request.Email, user.Email, cancellationToken);
            await ValidateDepartmentId(request.Position, request.DepartmentId, cancellationToken);
            await ValidateResourceGroupId(
                request.Position, request.ResourceGroupId, request.DepartmentId,
                user.Position, user.ResourceGroupId, user.DepartmentId,
                cancellationToken);
            await CheckIfRoleIsOccupied(request.Position, user.ResourceGroupId, request.ResourceGroupId, request.DepartmentId, cancellationToken);
            await ValidateInternPositionUpdate(user, request.Position, cancellationToken);

            var result = await _isManagerUserUpdateValidBusinessRule
                .IsCorrespondsToBusiness(new IsManagerUserUpdateValidBusinessRule.Request(
                    request.Position,
                    request.DepartmentId,
                    request.ResourceGroupId,
                    user.Position!,
                    user.DepartmentId),
                    cancellationToken);

            if (!result.IsSuccess)
            {
                throw new ActionNotAllowedException(result.Reason!);
            }
        }

        private void CheckAllowedPosition(UserDto user, string newPosition)
        {
            if (newPosition == user.Position)
            {
                return;
            }

            var hasManageHrClaim = _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name);
            if(hasManageHrClaim &&
                user.Position == PortalRoleNames.Intern &&
                newPosition == PortalRoleNames.Employee)
            {
                return;
            }

            var supportedPositionsToUpdate = new Dictionary<string, string[]>()
            {   // authed as              allowed position to set
                [PortalRoleNames.Ceo] = [PortalRoleNames.HeadOfDepartment, PortalRoleNames.GroupManager, PortalRoleNames.SystemAdministrator, PortalRoleNames.Employee, PortalRoleNames.Intern],
                [PortalRoleNames.Cto] = [PortalRoleNames.HeadOfDepartment, PortalRoleNames.GroupManager, PortalRoleNames.SystemAdministrator, PortalRoleNames.Employee, PortalRoleNames.Intern],
                [PortalRoleNames.HeadOfDepartment] = [PortalRoleNames.GroupManager, PortalRoleNames.Employee, PortalRoleNames.Intern],
                [PortalRoleNames.GroupManager] = [PortalRoleNames.Employee],
                [PortalRoleNames.Employee] = [],
                [PortalRoleNames.Intern] = [],
                [PortalRoleNames.SystemAdministrator] = [],
            };

            var authRoles = _authenticatedUserContext.GetRoles();
            if (!supportedPositionsToUpdate[authRoles.First()].Contains(newPosition))
            {
                throw new ActionNotAllowedException("You cannot change position to " + newPosition);
            }

            if (authRoles.IsHeadOfDepartment() && user.DepartmentId != _authenticatedUserContext.DepartmentId)
            {
                throw new ActionNotAllowedException("You cannot change position to " + newPosition);
            }

            if (authRoles.IsGroupManager() && user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
            {
                throw new ActionNotAllowedException("You cannot change position to " + newPosition);
            }
        }

        private async Task CheckIfRoleIsOccupied(
            string? requestPosition,
            int? userResourceGroupId,
            int? requestResourceGroupId,
            string? requestDepartmentId,
            CancellationToken cancellationToken)
        {
            if (requestPosition.IsGroupManager() && requestResourceGroupId != userResourceGroupId)
            {
                var canRoleBeOccupiedBusinessRuleModel = new CanRoleBeOccupiedBusinessRule.Request(requestPosition, requestDepartmentId, requestResourceGroupId);

                var result = await _canRoleBeOccupiedBusinessRule.IsCorrespondsToBusiness(canRoleBeOccupiedBusinessRuleModel, cancellationToken);

                if (!result.IsSuccess)
                {
                    throw new ActionNotAllowedException(result.Reason!);
                }
            }
        }

        private async Task ValidateRequestEmail(string? email, string currentEmail, CancellationToken cancellationToken)
        {
            if (email is null || email == currentEmail)
            {
                return;
            }

            var normalizedEmail = email.UpperInvariantNormalize();
            var isUserExistByEmail = await _identityDbContext.Users
                .AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
            if (isUserExistByEmail)
            {
                throw new EntityConflictException(string.Format(
                    CultureInfo.InvariantCulture,
                    PortalMessages.UserWithEmailAlreadyExists,
                    email
                ));
            }
        }

        private async Task ValidateDepartmentId(string? position, string? departmentId, CancellationToken cancellationToken)
        {
            if (_authenticatedUserContext.GetRoles().IsSystemAdministrator() && departmentId is null)
            {
                return;
            }

            var canRoleHaveDepartmentBusinessRuleRequest = new CanRoleHaveDepartmentBusinessRule.Request(position, departmentId);
            var canRoleHaveDepartmentBusinessRuleResult = await _canRoleHaveDepartmentBusinessRule.IsCorrespondsToBusiness(canRoleHaveDepartmentBusinessRuleRequest, cancellationToken);
            if (!canRoleHaveDepartmentBusinessRuleResult.IsSuccess)
            {
                throw new InvalidDataAppException(canRoleHaveDepartmentBusinessRuleResult.Reason!);
            }
        }

        private async Task ValidateResourceGroupId(
            string? requestPosition,
            int? requestResourceGroupId,
            string? requestDepartmentId,
            string? userPosition,
            int? userResourceGroupId,
            string? userDepartmentId,
            CancellationToken cancellationToken)
        {
            var isEmployeeWithoutGroup = userPosition!.IsEmployee() && requestResourceGroupId == null;
            var isInternWithoutGroup = userPosition!.IsIntern() && requestResourceGroupId == null;
            var isGroupManagerWithoutGroup = userPosition!.IsGroupManager() && requestResourceGroupId == null;

            var isChiefOrHeadWithResourceGroup = userPosition == requestPosition && (
                userPosition.IsHeadOfDepartment() ||
                    userPosition.IsCeo() ||
                    userPosition.IsCto()) &&
                requestResourceGroupId != null;

            var isRequestMakeChiefOrHeadWithResourceGroup =
                (requestPosition.IsHeadOfDepartment() ||
                    requestPosition.IsCeo() ||
                    requestPosition.IsCto()) &&
                requestResourceGroupId != null;

            if (isChiefOrHeadWithResourceGroup || isRequestMakeChiefOrHeadWithResourceGroup)
            {
                throw new ActionNotAllowedException(PortalMessages.UnsupportedUserUpdate);
            }

            if (requestResourceGroupId is null ||
                isGroupManagerWithoutGroup ||
                isEmployeeWithoutGroup ||
                isInternWithoutGroup ||
                userResourceGroupId == requestResourceGroupId &&
                userDepartmentId == requestDepartmentId)
            {
                return;
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager() || _authenticatedUserContext.GetRoles().IsSystemAdministrator())
            {
                throw new ActionNotAllowedException(PortalMessages.GmAndSysAdminCannotUpdateResourceGroup);
            }

            var canUserWithResourceGroupBeCreatedModel = new CanUserWithResourceGroupBeCreatedBusinessRule.Request(
                requestDepartmentId, requestResourceGroupId.Value, requestPosition!);

            var result = await _canUserWithResourceGroupBeCreatedBusinessRule
                .IsCorrespondsToBusiness(canUserWithResourceGroupBeCreatedModel, cancellationToken);

            if (!result.IsSuccess)
            {
                throw new ActionNotAllowedException(result.Reason!);
            }
        }

        private async Task ValidateInternPositionUpdate(UserDto user, string newPosition, CancellationToken cancellationToken)
        {
            if (user.Position == newPosition || user.Position != PortalRoleNames.Intern)
            {
                return;
            }

            var contract = await _teamBoardContract.GetUserContractDates(user.Id, cancellationToken);
            if (contract is null || !contract.ContractStartDate.HasValue || !contract.ContractEndDate.HasValue)
            {
                throw new ActionNotAllowedException($"Before updating the intern's position, the contract information must be completed");
            }
        }
    }
}
