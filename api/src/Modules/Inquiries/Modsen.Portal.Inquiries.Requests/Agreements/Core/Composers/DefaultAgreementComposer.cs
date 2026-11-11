using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;

internal interface IDefaultAgreementsComposer
{
    Task<List<UserAgreementEntity>> Compose(
        int userId,
        DateOnly agreementDateFrom,
        CancellationToken cancellationToken);
}

internal class DefaultAgreementsComposer : IDefaultAgreementsComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public DefaultAgreementsComposer(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<List<UserAgreementEntity>> Compose(
        int userId,
        DateOnly agreementDateFrom,
        CancellationToken cancellationToken)
    {
        var newAgreements = new List<UserAgreementEntity>();

        // default inquiry templates (absences) with default agreements
        var defaultInquiryTemplates = await _inquiriesDbContext.InquiryTemplates
            .Where(x => x.IsActive)
            .Where(x => x.IsDefault)
            .Where(x => UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.InquiryType))
            .Select(x => new
            {
                InquiryTemplate = x,
                DefaultAgreementTemplate = x.InquiryTemplateAgreements
                    .Where(x => !x.IsDeleted)
                    .FirstOrDefault(x => x.IsDefault),
            })
            .Where(x => x.DefaultAgreementTemplate != null)
            .ToListAsync(cancellationToken);

        // user agreements
        var existingUserAgreements = await _inquiriesDbContext.UserAgreements
            .Where(x => x.UserId == userId)
            .Where(x => !x.IsDeleted)
            .Include(x => x.InquiryTemplateAgreement)
            .ToListAsync(cancellationToken);

        // inquiry templates which is/was assigned to user
        var existingInquiryTemplateIds = existingUserAgreements
            .Select(x => x.InquiryTemplateAgreement!.InquiryTemplateId)
            .Distinct()
            .ToList();

        foreach (var defaultInquiryTemplate in defaultInquiryTemplates)
        {
            var agreement = defaultInquiryTemplate.DefaultAgreementTemplate!;

            // user already has/had agreement of this inquiry
            if (existingInquiryTemplateIds.Contains(defaultInquiryTemplate.InquiryTemplate.Id))
            {
                continue;
            }

            var newUserAgreement = new UserAgreementEntity
            {
                UserId = userId,
                DateFrom = agreementDateFrom,
                DateTo = null,
                InquiryAgreementTemplateId = agreement.Id,
                InquiryTemplateAgreement = agreement
            };

            newAgreements.Add(newUserAgreement);
        }

        return newAgreements;
    }
}
