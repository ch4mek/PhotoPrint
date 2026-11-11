using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface IGetAbsentApproversResolver
{
    Task<List<int>> ResolveAsync(ICollection<int>? approverIds, CancellationToken cancellationToken);
}

internal class GetAbsentApproversResolver : IGetAbsentApproversResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IClockService _clockService;

    public GetAbsentApproversResolver(IInquiriesDbContext inquiriesDbContext, IClockService clockService)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _clockService = clockService;
    }

    public Task<List<int>> ResolveAsync(ICollection<int>? approverIds, CancellationToken cancellationToken)
    {
        var query = _inquiriesDbContext.UserInquiries
            .AsNoTracking()
            .Where(x => x.InquiryTemplate!.ActivityType == ActivityType.NonWorkingPaid
                    || x.InquiryTemplate.ActivityType == ActivityType.NonWorkingUnpaid)
            .Where(x => x.Status == UserInquiryStatus.Approved)
            .Where(x => x.DateFrom <= _clockService.TodayUtc() && _clockService.TodayUtc() <= x.DateTo);

        if (approverIds != null)
        {
            query = query.Where(x => approverIds.Contains(x.UserId));
        }

        return query.Select(x => x.UserId)
            .ToListAsync(cancellationToken);
    }
}
