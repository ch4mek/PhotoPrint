using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class GetInquiriesByUserIdRequest : IQueryHandler<GetInquiriesByUserIdRequest.Query, List<UserInquiryCoreDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUserInquiryLoader _userInquiryLoader;
    private readonly IHasUserAccessToInquiryBusinessRule _hasUserAccessToInquiryBusinessRule;
    private readonly ISetAvailabilityToApproveRejectResolver _setAvailabilityToApproveRejectResolver;

    public GetInquiriesByUserIdRequest(
        IInquiriesDbContext inquiriesDbContext,
        IUserInquiryLoader userInquiryLoader,
        IHasUserAccessToInquiryBusinessRule hasUserAccessToInquiryBusinessRule,
        ISetAvailabilityToApproveRejectResolver setAvailabilityToApproveRejectResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _userInquiryLoader = userInquiryLoader;
        _hasUserAccessToInquiryBusinessRule = hasUserAccessToInquiryBusinessRule;
        _setAvailabilityToApproveRejectResolver = setAvailabilityToApproveRejectResolver;
    }

    public async Task<List<UserInquiryCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!await _hasUserAccessToInquiryBusinessRule.IsCorrespondsToBusiness(request.UserId,
            AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant, cancellationToken))
        {
            throw new ActionNotAllowedException($"You can't get inquiry for user: {request.UserId}");
        }

        var inquiryIds = await _inquiriesDbContext.UserInquiries
             .Where(x => x.UserId == request.UserId)
             .Select(x => x.Id)
             .ToArrayAsync(cancellationToken);      

        var inquiries = await _userInquiryLoader.Load(inquiryIds, cancellationToken);

        await _setAvailabilityToApproveRejectResolver.ResolveAsync(inquiries, null, cancellationToken);

        return inquiries;
    }

    internal sealed record Query(int UserId) : IQuery<List<UserInquiryCoreDto>>;
}
