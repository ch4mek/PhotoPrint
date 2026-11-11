using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;

internal interface IIsUserApproverBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int inquiryId, CancellationToken cancellationToken);
}

internal class IsUserApproverBusinessRule : IIsUserApproverBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public IsUserApproverBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IInquiriesDbContext inquiriesDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int inquiryId, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserInquiryAccepts
            .Where(accept => accept.UserInquiryId == inquiryId)
            .Where(accept => accept.AcceptUserId == _authenticatedUserContext.UserId
                        || accept.AcceptedById == _authenticatedUserContext.UserId
                        || accept.RejectedById == _authenticatedUserContext.UserId)
            .AnyAsync(cancellationToken);
    }
}
