using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core;

internal interface INotifySubstitutesOnApproverAbsenceService
{
    Task NotifyAsync(int absentUserId, CancellationToken cancellationToken);
}

internal class NotifySubstitutesOnApproverAbsenceService : INotifySubstitutesOnApproverAbsenceService
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IGetSubstitutesResolver _getSubstitutesResolver;
    private readonly INotificationSender _notificationSender;
    private readonly IUserInquiryLoader _inquiryLoader;

    public NotifySubstitutesOnApproverAbsenceService(
        IInquiriesDbContext inquiriesDbContext,
        IGetSubstitutesResolver getSubstitutesResolver,
        INotificationSender notificationSender,
        IUserInquiryLoader inquiryLoader)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _getSubstitutesResolver = getSubstitutesResolver;
        _notificationSender = notificationSender;
        _inquiryLoader = inquiryLoader;
    }

    public async Task NotifyAsync(int absentUserId, CancellationToken cancellationToken)
    {
        var pendingAccepts = await _inquiriesDbContext.UserInquiryAccepts
            .AsNoTracking()
            .Include(x => x.UserInquiryAgreementAssign)
                .ThenInclude(x => x!.InquiryTemplate)
            .Where(x => x.AcceptUserId == absentUserId)
            .Where(x => x.Status == UserInquiryAcceptStatus.WaitingForApprove)
            .ToListAsync(cancellationToken);

        if (pendingAccepts.Count == 0)
        {
            return;
        }

        var notifyDtos = pendingAccepts
            .Select(a => new InquiryUserNotifyDto
            {
                UserInquiryId = a.UserInquiryId!.Value,
                AcceptUserId = a.AcceptUserId,
                CreatedForUserId = a.UserInquiryAgreementAssign!.UserId
            })
            .ToList();

        var substituteNotifyDtos = await _getSubstitutesResolver.ResolveAsync(notifyDtos, cancellationToken);

        if (substituteNotifyDtos.Count == 0)
        {
            return;
        }

        var inquiryInfoByInquiryId = await _inquiryLoader
            .Load(pendingAccepts.Select(x => x.UserInquiryId!.Value).Distinct().ToArray(), cancellationToken);

        foreach (var substituteDto in substituteNotifyDtos)
        {
            var inquiry = inquiryInfoByInquiryId.FirstOrDefault(x => x.Id == substituteDto.UserInquiryId);

            if (inquiry == null)
            {
                continue;
            }

            await _notificationSender.SendInquiryAcceptNotification(
                substituteDto.CreatedForUserId!.Value,
                inquiry.Id,
                [substituteDto.AcceptUserId],
                cancellationToken);
        }
    }
}
