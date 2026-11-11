using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;

internal interface IIsAgreementValidForInquiryTemplateBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(InquiryType inquiryType, int templateId, int? agreementId, CancellationToken cancellationToken);
}

internal sealed class
    IsAgreementValidForInquiryTemplateBusinessRule : IIsAgreementValidForInquiryTemplateBusinessRule
{
    private readonly IInquiriesDbContext _dbContext;

    public IsAgreementValidForInquiryTemplateBusinessRule(IInquiriesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(
        InquiryType inquiryType,
        int templateId,
        int? agreementId,
        CancellationToken cancellationToken)
    {
        if (!UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(inquiryType))
        {
            return agreementId switch
            {
                null => true,
                _ => false
            };
        }

        switch (agreementId)
        {
            case null:
                return false;

            default:
                {
                    var isAgreementValid = await _dbContext.InquiryTemplateAgreements.AnyAsync(
                        agreement =>
                            agreement.Id == agreementId &&
                            agreement.InquiryTemplateId == templateId &&
                            !agreement.IsDeleted,
                        cancellationToken);

                    return isAgreementValid;
                }
        }


    }
}
