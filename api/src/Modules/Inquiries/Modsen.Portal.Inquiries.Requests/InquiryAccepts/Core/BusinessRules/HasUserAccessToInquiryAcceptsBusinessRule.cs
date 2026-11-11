using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.BusinessRules;


internal interface IHasUserAccessToInquiryAcceptsBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int inquiryId, CancellationToken cancellationToken);
}

internal class HasUserAccessToInquiryAcceptsBusinessRule : IHasUserAccessToInquiryAcceptsBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public HasUserAccessToInquiryAcceptsBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IInquiriesDbContext inquiriesDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int inquiryId, CancellationToken cancellationToken)
    {
        return _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name) ||
            _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ViewAllRequests.Name) ||
            await _inquiriesDbContext.UserInquiries
                .Where(x => x.Id == inquiryId)
                .AnyAsync(x => x.UserInquiryAccepts.Any(a => a.AcceptUserId == _authenticatedUserContext.UserId)
                || x.UserId == _authenticatedUserContext.UserId,
                cancellationToken);
    }
}
