using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Files.Core.BusinessRules;

internal interface IHasUserAccessToFilesBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int inquiryId, CancellationToken cancellationToken);
}

internal class HasUserAccessToFilesBusinessRule : IHasUserAccessToFilesBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public HasUserAccessToFilesBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IInquiriesDbContext inquiriesDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int inquiryId, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserInquiries
            .Where(x => x.Id == inquiryId)
            .AnyAsync(x => x.UserInquiryAccepts.Any(a => a.AcceptUserId == _authenticatedUserContext.UserId)
            || x.UserId == _authenticatedUserContext.UserId,
            cancellationToken);
    }
}
