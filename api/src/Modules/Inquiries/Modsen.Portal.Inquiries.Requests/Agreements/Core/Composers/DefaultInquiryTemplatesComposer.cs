using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;

internal interface IDefaultInquiryTemplatesComposer
{
    Task<List<UserInquiryTemplateEntity>> Compose(
        int userId,
        DateOnly dateFrom,
        DateOnly today,
        CancellationToken cancellationToken);
}

internal class DefaultInquiryTemplatesComposer : IDefaultInquiryTemplatesComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public DefaultInquiryTemplatesComposer(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<List<UserInquiryTemplateEntity>> Compose(int userId,
        DateOnly dateFrom,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var newUserInquiryTemplates = new List<UserInquiryTemplateEntity>();

        // default inquiry templates (absences)
        var defaultInquiryTemplates = await _inquiriesDbContext.InquiryTemplates
            .Where(x => x.IsActive)
            .Where(x => x.IsDefault)
            .Where(x => !UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.InquiryType))
            .ToListAsync(cancellationToken);

        // user inquiries
        var existingUserInquiryTemplates = await _inquiriesDbContext.UserInquiryTemplates
            .Where(x => x.AssignedToUserId == userId)
            .Where(x => x.DateTo == null || x.DateTo >= today)
            .ToListAsync(cancellationToken);

        // inquiry templates which is assigned to user
        var existingInquiryTemplateIds = existingUserInquiryTemplates
            .Select(x => x.InquiryTemplateId)
            .Distinct()
            .ToList();

        foreach (var defaultInquiryTemplate in defaultInquiryTemplates)
        {
            // user already has assign to this inquiry
            if (existingInquiryTemplateIds.Contains(defaultInquiryTemplate.Id))
            {
                continue;
            }

            var newUserInquiryTemplate = new UserInquiryTemplateEntity
            {
                AssignedToUserId = userId,
                DateFrom = dateFrom,
                DateTo = null,
                InquiryTemplateId = defaultInquiryTemplate.Id,
                InquiryTemplate = defaultInquiryTemplate,
            };

            newUserInquiryTemplates.Add(newUserInquiryTemplate);
        }

        return newUserInquiryTemplates;
    }
}
