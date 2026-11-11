using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Files.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Files.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Files.Requests;

internal sealed class GetFilesByInquiryIdRequest : IQueryHandler<GetFilesByInquiryIdRequest.Query, List<InquiryFileDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IHasUserAccessToFilesBusinessRule _hasUserAccessToFilesBusinessRule;

    public GetFilesByInquiryIdRequest(
        IInquiriesDbContext inquiriesDbContext,
        IHasUserAccessToFilesBusinessRule hasUserAccessToFilesBusinessRule)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _hasUserAccessToFilesBusinessRule = hasUserAccessToFilesBusinessRule;
    }

    public async Task<List<InquiryFileDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!await _hasUserAccessToFilesBusinessRule.IsCorrespondsToBusiness(request.InquiryId, cancellationToken))
        {
            return [];
        }

        var inquiryFileEntities = await _inquiriesDbContext.UserInquiryFiles
            .Where(x => x.UserInquiryId == request.InquiryId)
            .ToListAsync(cancellationToken);

        return inquiryFileEntities.Select(x => new InquiryFileDto
        {
            Id = x.Id,
            FileId = x.FileId,
            CreatedById = x.CreatedById,
            CreatedAtUtc = x.CreatedAtUtc,
            UpdatedById = x.UpdatedById,
            UpdatedAtUtc = x.UpdatedAtUtc
        }).ToList();
    }

    internal sealed record Query(int InquiryId) : IQuery<List<InquiryFileDto>>;
}
