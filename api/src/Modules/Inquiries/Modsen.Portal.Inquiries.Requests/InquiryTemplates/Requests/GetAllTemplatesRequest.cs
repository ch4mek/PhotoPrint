using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;

internal sealed class GetAllTemplatesRequest : ICommandHandler<GetAllTemplatesRequest.Command, List<InquiryTemplateDto>>
{
    private readonly IInquiryTemplateLoader _inquiryTemplateLoader;

    public GetAllTemplatesRequest(IInquiryTemplateLoader inquiryTemplateLoader)
    {
        _inquiryTemplateLoader = inquiryTemplateLoader;
    }

    public async Task<List<InquiryTemplateDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        return await _inquiryTemplateLoader.LoadAllAsync(cancellationToken);
    }

    internal sealed record Command : ICommand<List<InquiryTemplateDto>>, ITransactionalRequest { }
}
