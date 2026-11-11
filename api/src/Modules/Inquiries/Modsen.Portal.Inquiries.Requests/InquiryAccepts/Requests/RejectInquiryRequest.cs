using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;

internal sealed class RejectInquiryRequest : ICommandHandler<RejectInquiryRequest.Command, List<UserInquiryAcceptCoreDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUserInquiryAcceptStatusComposer _userInquiryAcceptStatusComposer;
    private readonly IInquiryAcceptLoader _inquiryAcceptLoader;
    private readonly IInquiryRejectedStatusResolver _inquiryRejectedStatusResolver;
    private readonly IUserInquiryAcceptToApproveRejectResolver _userInquiryAcceptToApproveRejectResolver;
    private readonly INotificationSender _notificationSender;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public RejectInquiryRequest(
        INotificationSender notificationSender,
        IInquiriesDbContext inquiriesDbContext,
        IUserInquiryAcceptStatusComposer userInquiryAcceptStatusComposer,
        IInquiryAcceptLoader inquiryAcceptLoader,
        IInquiryRejectedStatusResolver inquiryRejectedStatusResolver,
        IUserInquiryAcceptToApproveRejectResolver userInquiryAcceptToApproveRejectResolver,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _notificationSender = notificationSender;
        _inquiriesDbContext = inquiriesDbContext;
        _userInquiryAcceptStatusComposer = userInquiryAcceptStatusComposer;
        _inquiryAcceptLoader = inquiryAcceptLoader;
        _inquiryRejectedStatusResolver = inquiryRejectedStatusResolver;
        _userInquiryAcceptToApproveRejectResolver = userInquiryAcceptToApproveRejectResolver;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<UserInquiryAcceptCoreDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        var userInquiry = await _inquiriesDbContext.UserInquiries
            .Include(x => x.InquiryTemplate)
            .Include(x => x.UserInquiryAccepts)
            .Include(x => x.User)
            .Include(x => x.CreatedByUser)
            .AsSplitQuery()
            .FirstOrDefaultAsync(inquiry => inquiry.Id == request.InquiryId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserInquiryNotFound);

        var isRequester = _authenticatedUserContext.GetRequiredUserId() == userInquiry.UserId; //user rejects their own inquiry

        var inquiryAcceptEntities = userInquiry.UserInquiryAccepts;

        var inquiryAcceptsToReject = await _userInquiryAcceptToApproveRejectResolver
            .ResolveAsync(userInquiry, action: UserInquiryAcceptStatus.Rejected, cancellationToken);

        if (inquiryAcceptsToReject.Length != 0)
        {

            if (isRequester)
            {
                _userInquiryAcceptStatusComposer.Compose(inquiryAcceptsToReject[0], UserInquiryAcceptStatus.Cancelled);
            }
            else
            {
                _userInquiryAcceptStatusComposer.Compose(inquiryAcceptsToReject[0], UserInquiryAcceptStatus.Rejected);
            }
        }

        var pendingEntities = inquiryAcceptEntities
            .Where(accept => accept.Status is UserInquiryAcceptStatus.Pending or UserInquiryAcceptStatus.WaitingForApprove)
            .ToList();

        foreach (var accept in pendingEntities)
        {
            accept.Status = UserInquiryAcceptStatus.Cancelled;
        }

        userInquiry = isRequester
            ? await _inquiryRejectedStatusResolver.ResolveOwnInquiryAsync(userInquiry, cancellationToken)
            : await _inquiryRejectedStatusResolver.ResolveAsync(userInquiry, cancellationToken);

        await _notificationSender.SendInquiryRejectedNotification(
            templateName: userInquiry.InquiryTemplate!.Name,
            userInquiry.UserId,
            userInquiry.Id,
            cancellationToken);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return await _inquiryAcceptLoader.Load(inquiryAcceptsToReject.Select(x => x.Id).ToArray(), cancellationToken);
    }

    internal sealed record Command(int InquiryId)
        : ICommand<List<UserInquiryAcceptCoreDto>>, ITransactionalRequest;
}
