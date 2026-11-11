using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Core.Composers;

internal interface IResetUserAgreementsComposer
{
    Task<ResetUserAgreementsComposerDto> Compose(
        int[] userIds,
        InquiryTemplateAgreementEntity agreementTemplate,
        CancellationToken cancellationToken);
}

internal class ResetUserAgreementsComposer : IResetUserAgreementsComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public ResetUserAgreementsComposer(
        IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<ResetUserAgreementsComposerDto> Compose(
        int[] userIds,
        InquiryTemplateAgreementEntity agreementTemplate,
        CancellationToken cancellationToken)
    {
        var userAgreementsToRemove = new List<UserAgreementEntity>();
        var userAgreementTransactionsToRemove = new List<UserAgreementTransactionEntity>();

        foreach (var userId in userIds)
        {
            userAgreementsToRemove.AddRange(await _inquiriesDbContext.UserAgreements
                .Include(x => x.InquiryTemplateAgreement)
                .ThenInclude(x => x!.InquiryTemplate)
                .Where(x => x.UserId == userId &&
                    x.InquiryTemplateAgreement!.InquiryTemplateId != agreementTemplate.InquiryTemplateId &&
                    x.InquiryTemplateAgreement!.InquiryTemplate!.InquiryType == agreementTemplate.InquiryTemplate!.InquiryType)
                .ToListAsync(cancellationToken));

            userAgreementTransactionsToRemove.AddRange(await _inquiriesDbContext.UserAgreementTransactions
                .Include(x => x.UserAgreement)
                .ThenInclude(x => x!.InquiryTemplateAgreement)
                .ThenInclude(x => x!.InquiryTemplate)
                .Where(x => x.UserId == userId &&
                    x.UserAgreement!.InquiryTemplateAgreement!.InquiryTemplateId != agreementTemplate.InquiryTemplateId &&
                    x.UserAgreement!.InquiryTemplateAgreement!.InquiryTemplate!.InquiryType ==
                    agreementTemplate.InquiryTemplate!.InquiryType)
                .ToListAsync(cancellationToken));
        }
        return new ResetUserAgreementsComposerDto
        {
            UserAgreementsToRemove = userAgreementsToRemove,
            UserAgreementTransactionsToRemove = userAgreementTransactionsToRemove
        };
    }
}

internal record ResetUserAgreementsComposerDto
{
    public List<UserAgreementEntity> UserAgreementsToRemove { get; set; } = [];
    public List<UserAgreementTransactionEntity> UserAgreementTransactionsToRemove { get; set; } = [];
}
