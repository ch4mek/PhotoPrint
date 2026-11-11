using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.Queries;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;

internal sealed class NotifyInquiryApproverRequest : ICommandHandler<NotifyInquiryApproverRequest.Command, int>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IGetInquiryAcceptsByInquiryIdDbQuery _getInquiryAcceptsByIdQuery;
    private readonly IGetAbsentApproversResolver _getAbsentApproversResolver;
    private readonly IGetSubstituteResolver _getSubstituteResolver;
    private readonly IHasUserAccessBusinessRule _hasUserAccessBusinessRule;
    private readonly INotificationSender _notificationSender;

    public NotifyInquiryApproverRequest(
        IInquiriesDbContext inquiriesDbContext,
        IGetInquiryAcceptsByInquiryIdDbQuery getInquiryAcceptsByIdQuery,
        IGetAbsentApproversResolver getAbsentApproversResolver,
        IGetSubstituteResolver getSubstituteResolver,
        IHasUserAccessBusinessRule hasUserAccessBusinessRule,
        INotificationSender notificationSender)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _getInquiryAcceptsByIdQuery = getInquiryAcceptsByIdQuery;
        _getAbsentApproversResolver = getAbsentApproversResolver;
        _getSubstituteResolver = getSubstituteResolver;
        _hasUserAccessBusinessRule = hasUserAccessBusinessRule;
        _notificationSender = notificationSender;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var inquiryAcceptEntities = await _getInquiryAcceptsByIdQuery.ExecuteAsync(request.InquiryId, cancellationToken);

        var inquiryAcceptEntity = inquiryAcceptEntities
                .FirstOrDefault(userInquiryAcceptEntity => userInquiryAcceptEntity.Id == request.InquiryAcceptId)
            ?? throw new EntityNotFoundException($"Inquiry accept by id {request.InquiryAcceptId} in inquiry with id {request.InquiryId} not found");

        var userInquiry = await GetUserInquiry(request.InquiryId, cancellationToken);

        var hasUserAccessToNotifyApprover = await _hasUserAccessBusinessRule.IsCorrespondsToBusiness(
            request.InquiryId, userInquiry.UserId,
            AccessOptions.Manager | AccessOptions.Approver | AccessOptions.Substitute | AccessOptions.Hr | AccessOptions.RequestViewer,
            cancellationToken);

        if (inquiryAcceptEntity.Status != UserInquiryAcceptStatus.WaitingForApprove
                && inquiryAcceptEntity.Status != UserInquiryAcceptStatus.Pending
            || !hasUserAccessToNotifyApprover)
        {
            throw new ActionNotAllowedException("You can't notify this approver");
        }

        var isApproverAbsent = (await _getAbsentApproversResolver
            .ResolveAsync([inquiryAcceptEntity.AcceptUserId], cancellationToken))
            .Count != 0;

        var userIdsToNotify = new List<int> { inquiryAcceptEntity.AcceptUserId };

        if (isApproverAbsent)
        {
            var substituteId = await _getSubstituteResolver.ResolveAsync(userInquiry.InquiryTemplateId,
                userInquiry.UserId, inquiryAcceptEntity.AcceptPolicyScope, inquiryAcceptEntity.AcceptUserId,
                cancellationToken);

            if (substituteId.HasValue)
            {
                userIdsToNotify.Add(substituteId!.Value);
            }
        }

        await _notificationSender.SendInquiryAcceptNotification(
            userInquiry.UserId,
            userInquiry.Id,
            userIdsToNotify,
            cancellationToken);

        return 1;
    }

    private async Task<UserInquiryEntity> GetUserInquiry(int inquiryId, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserInquiries
                .Include(inquiry => inquiry.InquiryTemplate)
                .Include(inquiry => inquiry.User)
                .FirstOrDefaultAsync(inquiry => inquiry.Id == inquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"User inquiry by id {inquiryId} not found");
    }

    internal sealed record Command(int InquiryId, int InquiryAcceptId)
        : ICommand<int>, ITransactionalRequest;
}
