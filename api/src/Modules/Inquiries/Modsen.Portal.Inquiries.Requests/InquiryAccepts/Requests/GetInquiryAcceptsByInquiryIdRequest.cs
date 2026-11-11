using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.BusinessRules;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;

internal sealed class GetInquiryAcceptsByInquiryIdRequest : IQueryHandler<GetInquiryAcceptsByInquiryIdRequest.Query, List<UserInquiryAcceptCoreDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IInquiryAcceptLoader _inquiryAcceptLoader;
    private readonly IHasUserAccessToInquiryAcceptsBusinessRule _hasUserAccessToInquiryAcceptsBusinessRule;

    public GetInquiryAcceptsByInquiryIdRequest(
        IInquiriesDbContext inquiriesDbContext,
        IInquiryAcceptLoader inquiryAcceptLoader,
        IAuthenticatedUserContext authenticatedUserContext,
        IHasUserAccessToInquiryAcceptsBusinessRule hasUserAccessToInquiryAcceptsBusinessRule)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _inquiryAcceptLoader = inquiryAcceptLoader;
        _hasUserAccessToInquiryAcceptsBusinessRule = hasUserAccessToInquiryAcceptsBusinessRule;
    }

    public async Task<List<UserInquiryAcceptCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!await _inquiriesDbContext.UserInquiries.AnyAsync(x => x.Id == request.InquiryId, cancellationToken))
        {
            throw new EntityNotFoundException($"Inquiry with id {request.InquiryId} not found");
        }

        if (!await _hasUserAccessToInquiryAcceptsBusinessRule.IsCorrespondsToBusiness(request.InquiryId, cancellationToken))
        {
            throw new ActionNotAllowedException($"You can't get accepts for inquiry: {request.InquiryId}");
        }

        var inquiryAcceptIds = await _inquiriesDbContext.UserInquiryAccepts
            .Where(x => x.UserInquiryId == request.InquiryId)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return await _inquiryAcceptLoader.Load(inquiryAcceptIds, cancellationToken);
    }

    internal sealed record Query(int InquiryId) : IQuery<List<UserInquiryAcceptCoreDto>>;
}
