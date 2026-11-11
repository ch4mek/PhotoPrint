using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.BusinessRules;

internal interface IAreAcceptPoliciesValidBusinessRule : IBusinessRuleWithFailReason<AreAcceptPoliciesValidBusinessRule.Request>;

internal sealed class AreAcceptPoliciesValidBusinessRule : IAreAcceptPoliciesValidBusinessRule
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public AreAcceptPoliciesValidBusinessRule(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = ValidateCounts(request);
        if (result.IsSuccess)
        {
            result = await ValidatePolicies(request);
            if (result.IsSuccess)
            {
                result = ValidateDuplicates(request);
                if (result.IsSuccess)
                {
                    var isOrderWeightDuplicated = request.AcceptPolices
                        .GroupBy(acceptPolicy => acceptPolicy.OrderWeight)
                        .Any(group => group.Count() > 1);
                    var isOrderWeightInvalid = request.AcceptPolices
                        .Any(x => x.OrderWeight < 1);

                    result = isOrderWeightDuplicated || isOrderWeightInvalid
                        ? BusinessRuleResult.Fail("Incorrect order weight")
                        : BusinessRuleResult.Success();
                }
            }
        }

        return result;
    }

    private static BusinessRuleResult ValidateCounts(Request request)
    {
        if (request.AcceptPolices.Count == 0)
        {
            return BusinessRuleResult.Fail(PortalMessages.ApproversCountIsLessThanAllowed);
        }

        if (request.AcceptPolices.Count > 6)
        {
            return BusinessRuleResult.Fail(PortalMessages.ApproversCountIsMoreThanAllowed);
        }

        return BusinessRuleResult.Success();
    }

    private async Task<BusinessRuleResult> ValidatePolicies(Request request)
    {
        foreach (var acceptPolicy in request.AcceptPolices)
        {
            if (!ScopeIsValid(acceptPolicy))
            {
                return BusinessRuleResult.Fail("Invalid scope value");
            }

            if (!await UserIsActive(acceptPolicy.UserId) || !await UserIsActive(acceptPolicy.SubstituteUserId))
            {
                return BusinessRuleResult.Fail(PortalMessages.UserIsInactive);
            }

            var validationResult = ValidateAcceptPolicySubstitute(acceptPolicy, request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
        }

        return BusinessRuleResult.Success();
    }

    private static BusinessRuleResult ValidateAcceptPolicySubstitute(UpdateInquiryTemplateAcceptPolicyDto acceptPolicy,
        Request request)
    {
        if (request.IsSubstitutesToApproveAllowed)
        {
            return ValidateAcceptPolicySubstituteToApproveAllowed(acceptPolicy);
        }

        if (acceptPolicy.SubstituteScope is not null || acceptPolicy.SubstituteUserId is not null)
        {
            return BusinessRuleResult.Fail(PortalMessages.AcceptPolicyMustNotHasSubstituteScope);
        }

        return BusinessRuleResult.Success();
    }

    private static BusinessRuleResult ValidateAcceptPolicySubstituteToApproveAllowed(UpdateInquiryTemplateAcceptPolicyDto acceptPolicy)
    {
        if ((acceptPolicy.Scope != InquiryAcceptPolicyScopes.User && acceptPolicy.Scope == acceptPolicy.SubstituteScope) ||
            (acceptPolicy.UserId is not null && acceptPolicy.UserId == acceptPolicy.SubstituteUserId))
        {
            return BusinessRuleResult.Fail(PortalMessages.DuplicatedScopeAndSubstitute);
        }

        if (acceptPolicy is { SubstituteScope: InquiryAcceptPolicyScopes.User, SubstituteUserId: null })
        {
            return BusinessRuleResult.Fail(PortalMessages.AcceptPolicyMustHasSubstituteUser);
        }

        if (acceptPolicy.SubstituteScope != InquiryAcceptPolicyScopes.User &&
            acceptPolicy.SubstituteUserId is not null)
        {
            return BusinessRuleResult.Fail(PortalMessages.AcceptPolicyMustNotHasSubstituteUser);
        }

        return BusinessRuleResult.Success();
    }

    private static bool ScopeIsValid(UpdateInquiryTemplateAcceptPolicyDto acceptPolicy)
    {
        if (!(acceptPolicy.Scope == (InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO) ||
            acceptPolicy.Scope == InquiryAcceptPolicyScopes.HeadOfDepartment ||
            acceptPolicy.Scope == InquiryAcceptPolicyScopes.GroupManager ||
            acceptPolicy.Scope == InquiryAcceptPolicyScopes.ProjectManager ||
            acceptPolicy.Scope == InquiryAcceptPolicyScopes.User))
        {
            return false;
        }

        if (!(acceptPolicy.SubstituteScope == (InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO) ||
            acceptPolicy.SubstituteScope == InquiryAcceptPolicyScopes.HeadOfDepartment ||
            acceptPolicy.SubstituteScope == InquiryAcceptPolicyScopes.GroupManager ||
            acceptPolicy.SubstituteScope == InquiryAcceptPolicyScopes.User ||
            acceptPolicy.SubstituteScope == null))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> UserIsActive(int? userId)
    {
        if (userId != null)
        {
            var user = await _inquiriesDbContext.InquiriesUsers
                .FirstOrDefaultAsync(x => x.UserId == userId)
                ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

            if (!user.IsActive)
            {
                return user.IsActive;
            }
        }

        return true;
    }

    private static BusinessRuleResult ValidateDuplicates(Request request)
    {
        var baseApproverDuplicates = request.AcceptPolices
            .Where(acceptPolicy => acceptPolicy.Scope != InquiryAcceptPolicyScopes.User)
            .GroupBy(acceptPolicy => acceptPolicy.Scope)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (baseApproverDuplicates.Count > 0)
        {
            return BusinessRuleResult.Fail($"This approvers are duplicated: {string.Join(", ", baseApproverDuplicates)}");
        }

        var baseUserApproversDuplicated = request.AcceptPolices
            .Where(acceptPolicy => acceptPolicy.UserId != null)
            .GroupBy(acceptPolicy => acceptPolicy.UserId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (baseUserApproversDuplicated.Count > 0)
        {
            return BusinessRuleResult.Fail($"This user approvers are duplicated: {string.Join(", ", baseUserApproversDuplicated)}");
        }

        return BusinessRuleResult.Success();
    }

    internal sealed record Request
    {
        public bool IsSubstitutesToApproveAllowed { get; init; }
        public List<UpdateInquiryTemplateAcceptPolicyDto> AcceptPolices { get; init; } = [];
    }
}
