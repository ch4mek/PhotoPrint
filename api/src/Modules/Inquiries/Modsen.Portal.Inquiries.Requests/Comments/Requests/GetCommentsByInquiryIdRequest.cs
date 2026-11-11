using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Comments.Core;
using Modsen.Portal.Inquiries.Requests.Comments.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;

namespace Modsen.Portal.Inquiries.Requests.Comments.Requests
{
    internal sealed class GetCommentsByInquiryIdRequest : IQueryHandler<GetCommentsByInquiryIdRequest.Query, List<InquiryCommentCoreDto>>
    {
        private readonly IInquiriesDbContext _inquiriesDbContext;
        private readonly IHasUserAccessBusinessRule _hasUserAccessBusinessRule;
        private readonly IInquiryCommentsLoader _inquiryCommentsLoader;

        public GetCommentsByInquiryIdRequest(
            IInquiriesDbContext inquiriesDbContext,
            IHasUserAccessBusinessRule hasUserAccessBusinessRule,
            IInquiryCommentsLoader inquiryCommentsLoader)
        {
            _inquiriesDbContext = inquiriesDbContext;
            _hasUserAccessBusinessRule = hasUserAccessBusinessRule;
            _inquiryCommentsLoader = inquiryCommentsLoader;
        }

        public async Task<List<InquiryCommentCoreDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var hasUserAccess = await _hasUserAccessBusinessRule.IsCorrespondsToBusiness(request.InquiryId,
                AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Approver | AccessOptions.Substitute | AccessOptions.RequestViewer | AccessOptions.Coordinator,
                cancellationToken);

            if (!hasUserAccess)
            {
                return [];
            }

            var inquiryCommentIds = await _inquiriesDbContext.UserInquiryComments
                .Where(x => x.UserInquiryId == request.InquiryId)
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken);

            return await _inquiryCommentsLoader.Load(inquiryCommentIds, cancellationToken);
        }

        internal sealed record Query(int InquiryId) : IQuery<List<InquiryCommentCoreDto>>;
    }
}
