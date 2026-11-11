using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Comments.Core;
using Modsen.Portal.Inquiries.Requests.Comments.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Comments.Requests.RequestsDto;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Comments.Requests;

internal sealed class CreateInquiryCommentRequest : ICommandHandler<CreateInquiryCommentRequest.Command, InquiryCommentCoreDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IHasUserAccessBusinessRule _hasUserAccessBusinessRule;
    private readonly IInquiryCommentsLoader _inquiryCommentsLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly INotificationSender _notificationsSender;
    private readonly IGetAbsentApproversResolver _getAbsentApproversResolver;
    private readonly IGetSubstituteResolver _getSubstituteResolver;

    public CreateInquiryCommentRequest(
        IInquiriesDbContext inquiriesDbContext,
        IHasUserAccessBusinessRule hasUserAccessBusinessRule,
        IInquiryCommentsLoader inquiryCommentsLoader,
        IAuthenticatedUserContext authenticatedUserContext,
        INotificationSender notificationsSender,
        IGetAbsentApproversResolver getAbsentApproversResolver,
        IGetSubstituteResolver getSubstituteResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _hasUserAccessBusinessRule = hasUserAccessBusinessRule;
        _inquiryCommentsLoader = inquiryCommentsLoader;
        _authenticatedUserContext = authenticatedUserContext;
        _notificationsSender = notificationsSender;
        _getSubstituteResolver = getSubstituteResolver;
        _getAbsentApproversResolver = getAbsentApproversResolver;
    }

    public async Task<InquiryCommentCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var inquiry = await _inquiriesDbContext.UserInquiries
                .Include(inquiry => inquiry.UserInquiryAccepts)
                .Include(inquiry => inquiry.InquiryTemplate)
                .FirstOrDefaultAsync(inquiry => inquiry.Id == request.InquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry with Id {request.InquiryId} doesn't exist.");

        var hasUserAccess = await _hasUserAccessBusinessRule.IsCorrespondsToBusiness(request.InquiryId, inquiry.UserId,
            AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Approver | AccessOptions.Substitute | AccessOptions.RequestViewer | AccessOptions.Coordinator,
            cancellationToken);

        if (!hasUserAccess)
        {
            throw new ActionNotAllowedException($"You can't get leave a comment for inquiry: {request.InquiryId}");
        }

        var inquiryCommentFiles = request.Files
            .Select(x => new UserInquiryCommentFileEntity
            {
                FileId = x.FileId,
                FileName = x.FileName,
                FileSize = x.FileSize
            })
            .ToList();

        var inquiryCommentEntity = new UserInquiryCommentEntity
        {
            UserInquiryId = request.InquiryId,
            Comment = request.Comment,
            UserInquiryCommentFiles = inquiryCommentFiles
        };

        _inquiriesDbContext.UserInquiryComments.Add(inquiryCommentEntity);
        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        if (request.NotifyApprovers)
        {
            var usersIdsToNotify = await GetUsersIdsToNotify(inquiry, cancellationToken);

            await _notificationsSender.SendCommentCreatedNotification(
                _authenticatedUserContext.GetRequiredUserId(),
                inquiry.Id,
                inquiryCommentEntity.Id,
                usersIdsToNotify,
                cancellationToken);
        }

        return await _inquiryCommentsLoader.Load(inquiryCommentEntity.Id, cancellationToken);
    }

    private async Task<int[]> GetUsersIdsToNotify(UserInquiryEntity inquiry, CancellationToken cancellationToken)
    {
        if (inquiry.Status != UserInquiryStatus.Open)
        {
            return inquiry.UserId == _authenticatedUserContext.GetRequiredUserId() ? [] : [inquiry.UserId];
        }

        var userIdsToNotify = new List<int> { inquiry.UserId };

        var pendingAccepts = inquiry.UserInquiryAccepts.Where(accept => accept.Status
            is UserInquiryAcceptStatus.WaitingForApprove or
               UserInquiryAcceptStatus.Pending);

        foreach (var accept in pendingAccepts)
        {
            userIdsToNotify.Add(accept.AcceptUserId);

            var isApproverNotAbsent = (await _getAbsentApproversResolver.ResolveAsync(
                    [accept.AcceptUserId],
                    cancellationToken)
            ).Count == 0;

            if (isApproverNotAbsent)
            {
                continue;
            }

            var substituteId = await _getSubstituteResolver.ResolveAsync(
                inquiry.InquiryTemplateId,
                inquiry.UserId,
                accept.AcceptPolicyScope,
                accept.AcceptUserId,
                cancellationToken);

            if (substituteId.HasValue)
            {
                userIdsToNotify.Add(substituteId.Value);
            }
        }

        return userIdsToNotify
            .Where(id => id != _authenticatedUserContext.GetRequiredUserId())
            .Distinct()
            .ToArray();
    }

    internal sealed record Command(
        int InquiryId,
        string Comment,
        bool NotifyApprovers,
        IReadOnlyCollection<CreateInquiryCommentFileDto> Files)
        : ICommand<InquiryCommentCoreDto>, ITransactionalRequest;
}
