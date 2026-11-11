using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class GetInquiryByIdRequest : IQueryHandler<GetInquiryByIdRequest.Query, UserInquiryCoreDto>
{
    private readonly IUserInquiryLoader _userInquiryLoader;
    private readonly IHasUserAccessToInquiryBusinessRule _hasUserAccessToInquiryBusinessRule;
    private readonly IIsUserApproverBusinessRule _isUserApproverBusinessRule;
    private readonly IHistoricalRemainingBalanceDaysHoursResolver _remainingBalanceDaysHoursResolver;
    private readonly IAvailableInquiryAcceptsResolver _availableInquiryAcceptsResolver;
    private readonly IGetInquiriesCanBeApprovedAsSubstituteResolver _getInquiriesCanBeApprovedAsSubstituteResolver;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ISetUserInfoVisibilityResolver _setUserInfoVisibilityResolver;

    public GetInquiryByIdRequest(
        IUserInquiryLoader userInquiryLoader,
        IHasUserAccessToInquiryBusinessRule hasUserAccessToInquiryBusinessRule,
        IIsUserApproverBusinessRule isUserApproverBusinessRule,
        IHistoricalRemainingBalanceDaysHoursResolver remainingBalanceDaysHoursResolver,
        IAvailableInquiryAcceptsResolver availableInquiryAcceptsResolver,
        IGetInquiriesCanBeApprovedAsSubstituteResolver getInquiriesCanBeApprovedAsSubstituteResolver,
        IAuthenticatedUserContext authenticatedUserContext,
        ISetUserInfoVisibilityResolver setUserInfoVisibilityResolver)
    {
        _userInquiryLoader = userInquiryLoader;
        _hasUserAccessToInquiryBusinessRule = hasUserAccessToInquiryBusinessRule;
        _isUserApproverBusinessRule = isUserApproverBusinessRule;
        _remainingBalanceDaysHoursResolver = remainingBalanceDaysHoursResolver;
        _availableInquiryAcceptsResolver = availableInquiryAcceptsResolver;
        _getInquiriesCanBeApprovedAsSubstituteResolver = getInquiriesCanBeApprovedAsSubstituteResolver;
        _authenticatedUserContext = authenticatedUserContext;
        _setUserInfoVisibilityResolver = setUserInfoVisibilityResolver;
    }

    public async Task<UserInquiryCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var userInquiry = await _userInquiryLoader.Load(request.InquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry by id {request.InquiryId} not found");

        var isUserManager = await _hasUserAccessToInquiryBusinessRule.IsCorrespondsToBusiness(
            userInquiry.InquiryUserId, AccessOptions.Manager | AccessOptions.Accountant | AccessOptions.Coordinator,
            cancellationToken);

        var isUserApprover = await _isUserApproverBusinessRule.IsCorrespondsToBusiness(request.InquiryId,
            cancellationToken);

        var userIsNotManagerAndNotApprover = !isUserManager && !isUserApprover;
        var hasViewAllRequests = _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ViewAllRequests.Name);

        var isCanBeApprovedRejectedAsBySubstitute = (await _getInquiriesCanBeApprovedAsSubstituteResolver
                .ResolveAsync([userInquiry.Id], cancellationToken))
            .Count != 0;

        if (userIsNotManagerAndNotApprover && !isCanBeApprovedRejectedAsBySubstitute && !hasViewAllRequests)
        {
            throw new ActionNotAllowedException($"You can't get inquiry for user: {userInquiry.InquiryUserId}");
        }

        var availableToApproveAccepts = await _availableInquiryAcceptsResolver
            .ResolveForApprovalAsync(request.InquiryId, cancellationToken);

        var (_, isAbleToReject) = await _availableInquiryAcceptsResolver
            .ResolveForRejectionAsync(request.InquiryId, cancellationToken);

        userInquiry.CanCurrentUserApprove =
            availableToApproveAccepts.Count != 0 || isCanBeApprovedRejectedAsBySubstitute;
        userInquiry.CanCurrentUserReject = isAbleToReject || isCanBeApprovedRejectedAsBySubstitute;

        if (userInquiry.Status == UserInquiryStatus.Open)
        {
            await _remainingBalanceDaysHoursResolver.ResolveAsync(userInquiry, cancellationToken);
        }

        await _setUserInfoVisibilityResolver.ResolveAsync(userInquiry, AccessOptions.Self | AccessOptions.Manager |
            AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator, cancellationToken);

        return userInquiry;
    }

    internal sealed record Query(int InquiryId) : IQuery<UserInquiryCoreDto>;
}
