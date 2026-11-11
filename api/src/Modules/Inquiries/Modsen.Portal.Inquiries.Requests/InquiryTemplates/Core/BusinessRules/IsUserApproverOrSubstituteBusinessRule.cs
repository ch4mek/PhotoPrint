using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.BusinessRules;

internal interface IIsUserApproverOrSubstituteBusinessRule : IBusinessRule<IsUserApproverOrSubstituteBusinessRuleModel>;

internal record IsUserApproverOrSubstituteBusinessRuleModel(int UserId);

internal class IsUserApproverOrSubstituteBusinessRule : IIsUserApproverOrSubstituteBusinessRule
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public IsUserApproverOrSubstituteBusinessRule(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(IsUserApproverOrSubstituteBusinessRuleModel? model, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .Include(x => x.InquiryTemplate)
            .AnyAsync(x =>
                    (x.UserId == model!.UserId || x.SubstituteUserId == model.UserId) && x.InquiryTemplate!.IsActive,
                cancellationToken);
    }
}
