using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface IUserInquiryAcceptToApproveRejectResolver
{
    Task<UserInquiryAcceptEntity[]> ResolveAsync(UserInquiryEntity userInquiry, UserInquiryAcceptStatus action,
        CancellationToken cancellationToken);
}

internal class UserInquiryAcceptToApproveRejectResolver : IUserInquiryAcceptToApproveRejectResolver
{
    private readonly IAvailableInquiryAcceptsResolver _availableInquiryAcceptsResolver;
    private readonly IGetInquiriesCanBeApprovedAsSubstituteResolver _getInquiriesCanBeApprovedAsSubstituteResolver;
    private readonly IGetAbsentApproversResolver _getAbsentApproversResolver;

    public UserInquiryAcceptToApproveRejectResolver(IAvailableInquiryAcceptsResolver availableInquiryAcceptsResolver,
        IGetInquiriesCanBeApprovedAsSubstituteResolver getInquiriesCanBeApprovedAsSubstituteResolver,
        IGetAbsentApproversResolver getAbsentApproversResolver)
    {
        _availableInquiryAcceptsResolver = availableInquiryAcceptsResolver;
        _getInquiriesCanBeApprovedAsSubstituteResolver = getInquiriesCanBeApprovedAsSubstituteResolver;
        _getAbsentApproversResolver = getAbsentApproversResolver;
    }

    public async Task<UserInquiryAcceptEntity[]> ResolveAsync(UserInquiryEntity userInquiry, UserInquiryAcceptStatus action,
        CancellationToken cancellationToken)
    {
        if (action == UserInquiryAcceptStatus.Pending || action == UserInquiryAcceptStatus.WaitingForApprove)
        {
            throw new ArgumentException(null, nameof(action));
        }

        List<UserInquiryAcceptEntity> inquiryAcceptsToApproveReject;
        var isActionAvailable = false;

        if (action == UserInquiryAcceptStatus.Accepted)
        {
            inquiryAcceptsToApproveReject = _availableInquiryAcceptsResolver.ResolveForApproval(userInquiry);
            isActionAvailable = inquiryAcceptsToApproveReject.Count != 0;
        }
        else
        {
            var (acceptsToReject, isAbleToReject) = _availableInquiryAcceptsResolver.ResolveForRejection(userInquiry);
            inquiryAcceptsToApproveReject = acceptsToReject;
            isActionAvailable = isAbleToReject;
        }

        if (!isActionAvailable)
        {
            var canBeApprovedRejectedAsSubstitute = (await _getInquiriesCanBeApprovedAsSubstituteResolver
                .ResolveAsync([userInquiry.Id], cancellationToken))
                .Count != 0;

            if (!canBeApprovedRejectedAsSubstitute)
            {
                throw new ActionNotAllowedException("You cannot accept/reject the request");
            }

            var acceptUserIds = userInquiry.UserInquiryAccepts
                .Where(x => x.Status == UserInquiryAcceptStatus.WaitingForApprove)
                .Select(x => x.AcceptUserId)
                .ToArray();

            var absentApprovers = await _getAbsentApproversResolver.ResolveAsync(acceptUserIds, cancellationToken);

            return userInquiry.UserInquiryAccepts
                .Where(x => absentApprovers.Contains(x.AcceptUserId))
                .ToArray();
        }

        return inquiryAcceptsToApproveReject.Take(1).ToArray();
    }
}
