using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules
{
    internal interface IIsUserHasAccessToMutateConversation : IBusinessRuleWithFailReason<IsUserHasAccessToMutateConversation.Request>;

    internal class IsUserHasAccessToMutateConversation : IIsUserHasAccessToMutateConversation
    {
        private readonly ITeamBoardDbContext _teamBoardDbContext;
        private readonly IAuthenticatedUserContext _authenticatedUserContext;
        private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

        public IsUserHasAccessToMutateConversation(
            ITeamBoardDbContext teamBoardDbContext,
            IAuthenticatedUserContext authenticatedUserContext,
            ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
        {
            _teamBoardDbContext = teamBoardDbContext;
            _authenticatedUserContext = authenticatedUserContext;
            _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        }

        public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await _teamBoardDbContext.TeamBoardUsers
                .FirstOrDefaultAsync(x => x.UserId == model.UserId, cancellationToken)
                ?? throw new EntityNotFoundException($"There is no user with Id {model.UserId}");

            if (user.UserId == _authenticatedUserContext.UserId)
            {
                return BusinessRuleResult.Fail("You can't get your conversation information");
            }

            var canRetrieveAsHigherManager = !user.Position.IsCeo() && !user.Position.IsCto() &&
                await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(model.UserId, cancellationToken);
            var canRetrieveInternConversation = CanRetrieveInternConversation(user);
            var canRetrieveAsHR = _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name);

            if (!canRetrieveAsHigherManager && !canRetrieveAsHR && !canRetrieveInternConversation)
            {
                return BusinessRuleResult.Fail("");
            }

            var hrOnlyAccess = canRetrieveAsHR && !canRetrieveAsHigherManager && !canRetrieveInternConversation;

            if (hrOnlyAccess && model.ConversationType != ConversationType.HRConversation)
            {
                return BusinessRuleResult.Fail("HR have access only to conversations with 'HR Conversation' type");
            }            

            return ValidateHrConversation(model);
        }

        private bool CanRetrieveInternConversation(TeamBoardUser user)
        {
            if (user.Position.IsIntern())
            {
                if (_authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name))
                {
                    return true;
                }

                var hasManageInternsClaim = _authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name);

                if (hasManageInternsClaim)
                {
                    var currentUserRole = _authenticatedUserContext.GetRoles();

                    if (currentUserRole.IsInOneOfTheRoles(PortalRoleNames.Ceo, PortalRoleNames.Cto))
                    {
                        return true;
                    }
                    else if (currentUserRole.IsHeadOfDepartment())
                    {
                        return user.DepartmentId != null &&
                            _authenticatedUserContext.DepartmentId == user.DepartmentId;
                    }
                    else if (currentUserRole.IsGroupManager())
                    {
                        return user.ResourceGroupId != null &&
                            _authenticatedUserContext.ResourceGroupId == user.ResourceGroupId;
                    }
                }
            }

            return false;
        }

        private static BusinessRuleResult ValidateHrConversation(Request model)
        {
            if (model.ConversationType == ConversationType.HRConversation)
            {
                if (model.PerformanceReviewDate.HasValue)
                {
                    throw new InvalidDataAppException("HR Conversation can't have Performance Review Date");
                }

                if (model.UserPerformanceReviewId.HasValue)
                {
                    throw new InvalidDataAppException("HR Conversation can't have User Performance Review Id");
                }

                if (model.Passed.HasValue)
                {
                    throw new InvalidDataAppException("HR Conversation can't have Passed status");
                }

                if (model.Salaries.Count > 0)
                {
                    throw new InvalidDataAppException("HR Conversation can't have salary");
                }

                if (model.DevelopmentPlans.Count > 0)
                {
                    throw new InvalidDataAppException("HR Conversation can't have development plan");
                }
            }

            return BusinessRuleResult.Success();
        }

        public sealed record Request(
            int UserId,
            ConversationType ConversationType,
            DateTime? PerformanceReviewDate,
            int? UserPerformanceReviewId,
            bool? Passed,
            IReadOnlyCollection<UserSalaryFullCoreDto> Salaries,
            IReadOnlyCollection<DevelopmentPlanFullCoreDto> DevelopmentPlans);
    }
}
