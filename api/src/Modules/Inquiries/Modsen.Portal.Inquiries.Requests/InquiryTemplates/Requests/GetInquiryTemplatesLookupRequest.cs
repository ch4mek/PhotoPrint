using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;

internal sealed class GetInquiryTemplatesLookupRequest
    : ICommandHandler<GetInquiryTemplatesLookupRequest.Command, List<InquiryTemplatesLookupDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetInquiryTemplatesLookupRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public Task<List<InquiryTemplatesLookupDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        return _inquiriesDbContext.InquiryTemplates
            .Where(x => x.IsActive)
            .OrderBy(x => x.InquiryTypeOrderWeight)
            .Select(x => new InquiryTemplatesLookupDto
            {
                Id = x.Id,
                Name = x.Name,
                InquiryType = x.InquiryType
            }).ToListAsync(cancellationToken);
    }

    internal sealed record Command : ICommand<List<InquiryTemplatesLookupDto>>, ITransactionalRequest { }
}
