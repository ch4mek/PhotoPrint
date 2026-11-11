using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Core.Composers;

internal interface IUserInquiryTemplateComposer
{
    Task<List<UserInquiryTemplateEntity>> Compose(
        int[] userIds,
        DateOnly effectiveFrom,
        int inquiryTemplateId,
        CancellationToken cancellationToken);
}

internal sealed class UserInquiryTemplateComposer : IUserInquiryTemplateComposer
{
    private readonly IInquiriesDbContext _dbContext;

    public UserInquiryTemplateComposer(IInquiriesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserInquiryTemplateEntity>> Compose(
        int[] userIds,
        DateOnly effectiveFrom,
        int inquiryTemplateId,
        CancellationToken cancellationToken)
    {
        var templateAssigns = new List<UserInquiryTemplateEntity>();

        foreach (var userId in userIds)
        {
            var dateFrom = effectiveFrom;

            var previousTemplatesWithSameType = await _dbContext.UserInquiryTemplates
                .Include(assign => assign.InquiryTemplate)
                .Where(assign =>
                    assign.InquiryTemplate!.Id == inquiryTemplateId &&
                    assign.AssignedToUserId == userId &&
                    (assign.DateTo == null || assign.DateTo <= dateFrom))
                .ToListAsync(cancellationToken);

            foreach (var previousTemplate in previousTemplatesWithSameType)
            {
                previousTemplate.DateTo = dateFrom.AddDays(-1);
            }

            var assignment = new UserInquiryTemplateEntity
            {
                DateFrom = dateFrom,
                DateTo = null,
                AssignedToUserId = userId,
                InquiryTemplateId = inquiryTemplateId,
            };

            templateAssigns.Add(assignment);
        }

        return templateAssigns;
    }
}
