using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;

internal sealed class AcceptInquiryRequest : ICommandHandler<AcceptInquiryRequest.Command, List<UserInquiryAcceptCoreDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUserInquiryAcceptStatusComposer _userInquiryAcceptStatusComposer;
    private readonly INotificationSender _notificationSender;
    private readonly IInquiryAcceptLoader _inquiryAcceptLoader;
    private readonly IInquiryAcceptedStatusResolver _inquiryAcceptedStatusResolver;
    private readonly IUserInquiryAcceptToApproveRejectResolver _inquiryAcceptedToApproveRejectResolver;
    private readonly INextApproversToNotifyResolver _nextApproversToNotifyResolver;
    private readonly INotifySubstitutesOnApproverAbsenceService _notifySubstitutesOnApproverAbsenceService;
    private readonly IClockService _clockService;

    public AcceptInquiryRequest(
        IInquiriesDbContext inquiriesDbContext,
        IUserInquiryAcceptStatusComposer userInquiryAcceptStatusComposer,
        INotificationSender notificationSender,
        IInquiryAcceptLoader inquiryAcceptLoader,
        IInquiryAcceptedStatusResolver inquiryAcceptedStatusResolver,
        IUserInquiryAcceptToApproveRejectResolver inquiryAcceptedToApproveRejectResolver,
        INextApproversToNotifyResolver nextApproversToNotifyResolver,
        INotifySubstitutesOnApproverAbsenceService notifySubstitutesOnApproverAbsenceService,
        IClockService clockService)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _userInquiryAcceptStatusComposer = userInquiryAcceptStatusComposer;
        _notificationSender = notificationSender;
        _inquiryAcceptLoader = inquiryAcceptLoader;
        _inquiryAcceptedStatusResolver = inquiryAcceptedStatusResolver;
        _inquiryAcceptedToApproveRejectResolver = inquiryAcceptedToApproveRejectResolver;
        _nextApproversToNotifyResolver = nextApproversToNotifyResolver;
        _notifySubstitutesOnApproverAbsenceService = notifySubstitutesOnApproverAbsenceService;
        _clockService = clockService;
    }

    public async Task<List<UserInquiryAcceptCoreDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        var userInquiry = await GetUserInquiry(request.InquiryId, cancellationToken);
        var inquiryAcceptEntities = userInquiry.UserInquiryAccepts;

        var inquiryAcceptsToApprove = await _inquiryAcceptedToApproveRejectResolver
            .ResolveAsync(userInquiry, action: UserInquiryAcceptStatus.Accepted, cancellationToken);

        _userInquiryAcceptStatusComposer.Compose(inquiryAcceptsToApprove, UserInquiryAcceptStatus.Accepted);
        var (isInquiryApproved, nextAccepts) = GetNextAccepts(inquiryAcceptsToApprove[0], inquiryAcceptEntities);

        if (isInquiryApproved)
        {
            await _inquiryAcceptedStatusResolver.ResolveAsync(userInquiry, cancellationToken);

            await _notificationSender.SendInquiryApprovedNotification(
                templateName: userInquiry.InquiryTemplate!.Name,
                userInquiry.UserId,
                userInquiry.Id,
                cancellationToken);

            var today = _clockService.TodayUtc();
            if ((userInquiry.InquiryTemplate.ActivityType == ActivityType.NonWorkingPaid ||
                 userInquiry.InquiryTemplate.ActivityType == ActivityType.NonWorkingUnpaid) &&
                userInquiry.DateFrom <= today && today <= userInquiry.DateTo)
            {
                await _notifySubstitutesOnApproverAbsenceService.NotifyAsync(userInquiry.UserId, cancellationToken);
            }
        }
        else
        {
            nextAccepts.ForEach(x => x.Status = UserInquiryAcceptStatus.WaitingForApprove);
            var approverIds = await _nextApproversToNotifyResolver.ResolveAsync(nextAccepts, userInquiry, cancellationToken);

            await _notificationSender.SendInquiryAcceptNotification(
                userInquiry.UserId,
                userInquiry.Id,
                approverIds,
                cancellationToken);
        }

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return await _inquiryAcceptLoader.Load(inquiryAcceptsToApprove.Select(x => x.Id).ToArray(), cancellationToken);
    }

    private static (bool IsInquiryApproved, List<UserInquiryAcceptEntity> NextAccepts) GetNextAccepts(
        UserInquiryAcceptEntity inquiryAcceptEntity,
        List<UserInquiryAcceptEntity> inquiryAcceptEntities)
    {
        if (inquiryAcceptEntity.AcceptPolicyScope == InquiryAcceptPolicyScopes.ProjectManager)
        {
            var inquiryProjectManagerAccepts = inquiryAcceptEntities
                .Where(x => x.AcceptPolicyScope == InquiryAcceptPolicyScopes.ProjectManager);

            if (inquiryProjectManagerAccepts.All(x => x.Status == UserInquiryAcceptStatus.Accepted))
            {
                var nextAcceptEntity = inquiryAcceptEntities
                    .Where(accept => accept.Status == UserInquiryAcceptStatus.Pending)
                    .MinBy(accept => accept.Id);

                return (nextAcceptEntity == null, nextAcceptEntity != null ? [nextAcceptEntity] : []);
            }

            return (false, []);
        }

        var nextAccept = inquiryAcceptEntities
            .Where(accept => accept.Status == UserInquiryAcceptStatus.Pending)
            .MinBy(accept => accept.Id);

        if (nextAccept == null)
        {
            return (true, []);
        }

        var nextAccepts = nextAccept.AcceptPolicyScope == InquiryAcceptPolicyScopes.ProjectManager
            ? inquiryAcceptEntities
                .Where(accept => accept.AcceptPolicyScope == InquiryAcceptPolicyScopes.ProjectManager)
                .ToList()
            : [nextAccept];

        return (nextAccepts.Count == 0, nextAccepts);
    }

    private async Task<UserInquiryEntity> GetUserInquiry(int inquiryId, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserInquiries
            .Include(inquiry => inquiry.InquiryTemplate)
            .Include(inquiry => inquiry.User)
            .Include(inquiry => inquiry.CreatedByUser)
            .Include(inquiry => inquiry.UserInquiryAccepts)
            .AsSplitQuery()
            .FirstOrDefaultAsync(inquiry => inquiry.Id == inquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"User inquiry by id {inquiryId} not found");
    }

    internal sealed record Command(int InquiryId) : ICommand<List<UserInquiryAcceptCoreDto>>, ITransactionalRequest;
}
