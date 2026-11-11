using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface ISetAvailabilityToApproveRejectResolver
{
    Task<List<UserInquiryCoreDto>> ResolveAsync(List<UserInquiryCoreDto> inquiries,
        ICollection<int>? inquiryIdsCanBeApprovedAsSubstitute, CancellationToken cancellationToken);
}

internal class SetAvailabilityToApproveRejectResolver : ISetAvailabilityToApproveRejectResolver
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetInquiriesCanBeApprovedAsSubstituteResolver _getInquiriesCanBeApprovedAsSubstituteResolver;

    public SetAvailabilityToApproveRejectResolver(IAuthenticatedUserContext authenticatedUserContext,
        IGetInquiriesCanBeApprovedAsSubstituteResolver getInquiriesCanBeApprovedAsSubstituteResolver)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getInquiriesCanBeApprovedAsSubstituteResolver = getInquiriesCanBeApprovedAsSubstituteResolver;
    }

    public async Task<List<UserInquiryCoreDto>> ResolveAsync(List<UserInquiryCoreDto> inquiries,
        ICollection<int>? inquiryIdsCanBeApprovedAsSubstitute, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name)
            || _authenticatedUserContext.GetRoles().IsCeo() || _authenticatedUserContext.GetRoles().IsCto())
        {
            foreach (var inquiry in inquiries)
            {
                inquiry.CanCurrentUserApprove = inquiry.Status == UserInquiryStatus.Open;
                inquiry.CanCurrentUserReject = inquiry.Status != UserInquiryStatus.Rejected
                                            && inquiry.Status != UserInquiryStatus.Canceled;
            }

            return inquiries;
        }

        var inquiryIdsCanBeApprovedAsMainApprover = inquiries
            .Where(x => x.UserInquiryAccepts.Any(a => a.Status == UserInquiryAcceptStatus.WaitingForApprove
                && a.AcceptUser!.Id == _authenticatedUserContext.UserId))
            .Select(x => x.Id)
            .ToArray();

        inquiryIdsCanBeApprovedAsSubstitute ??= await _getInquiriesCanBeApprovedAsSubstituteResolver
                .ResolveAsync(onlyAsAnyUser: false, cancellationToken);

        foreach(var inquiry in inquiries)
        {
            inquiry.CanCurrentUserApprove = inquiryIdsCanBeApprovedAsSubstitute.Contains(inquiry.Id)
                || inquiryIdsCanBeApprovedAsMainApprover.Contains(inquiry.Id);

            inquiry.CanCurrentUserReject = inquiry.CanCurrentUserApprove
                || inquiry.CreatedForUser.Id == _authenticatedUserContext.UserId;
        }

        return inquiries;
    }
}
