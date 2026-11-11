using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;

internal sealed class GetTemplateByIdRequest : IQueryHandler<GetTemplateByIdRequest.Query, InquiryTemplateDto>
{
    private readonly IInquiryTemplateLoader _inquiryTemplateLoader;

    public GetTemplateByIdRequest(IInquiryTemplateLoader inquiryTemplateLoader)
    {
        _inquiryTemplateLoader = inquiryTemplateLoader;
    }

    public async Task<InquiryTemplateDto> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _inquiryTemplateLoader.LoadAsync(request.Id, cancellationToken);
    }

    internal sealed record Query(int Id) : IQuery<InquiryTemplateDto>, ITransactionalRequest;
}
