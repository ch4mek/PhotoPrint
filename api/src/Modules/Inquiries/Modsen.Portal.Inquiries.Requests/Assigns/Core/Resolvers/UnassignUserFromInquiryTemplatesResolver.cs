using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Core.Resolvers;

internal interface IUnassignUserFromInquiryTemplatesResolver
{
    Task ResolveAsync(int userId, int unassignById, CancellationToken cancellationToken);
}

internal class UnassignUserFromInquiryTemplatesResolver : IUnassignUserFromInquiryTemplatesResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IClockService _clockService;
    private readonly IRejectInquiriesAfterUnassignResolver _rejectInquiriesAfterUnassignResolver;

    public UnassignUserFromInquiryTemplatesResolver(
        IInquiriesDbContext inquiriesDbContext,
        IClockService clockService,
        IRejectInquiriesAfterUnassignResolver rejectInquiriesAfterUnassignResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _clockService = clockService;
        _rejectInquiriesAfterUnassignResolver = rejectInquiriesAfterUnassignResolver;
    }

    public async Task ResolveAsync(int userId, int unassignById, CancellationToken cancellationToken)
    {
        var today = _clockService.TodayUtc();

        // unassign user inquiry template assigns
        var assignsToDelete = await _inquiriesDbContext.UserInquiryTemplates
            .Where(x => x.AssignedToUserId == userId)
            .ToListAsync(cancellationToken);

        assignsToDelete.ForEach(x => x.DateTo = today.AddDays(-1));
        var inquiryTemplateIds = assignsToDelete.Select(x => x.InquiryTemplateId).Distinct();

        // delete user agreement assigns 
        var userAgreements = await _inquiriesDbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .Where(x => x.UserId == userId)
            .Include(x => x.InquiryTemplateAgreement)
            .ToListAsync(cancellationToken);

        userAgreements.ForEach(x => x.DateTo = x.DateFrom.AddDays(-1));
        userAgreements.ForEach(x => x.IsDeleted = true);
        var agreementInquiryTemplateIds = userAgreements.Select(x => x.InquiryTemplateAgreement!.InquiryTemplateId);

        await _rejectInquiriesAfterUnassignResolver.ResolveAsync(
            userId,
            inquiryTemplateIds.Concat(agreementInquiryTemplateIds).ToArray(),
            unassignById,
            cancellationToken);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);
    }
}
