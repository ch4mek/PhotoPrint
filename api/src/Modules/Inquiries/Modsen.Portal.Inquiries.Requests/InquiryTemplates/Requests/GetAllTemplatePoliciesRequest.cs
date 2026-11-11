using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;

internal sealed class GetAllTemplatePoliciesRequest :
    ICommandHandler<GetAllTemplatePoliciesRequest.Command, List<GetInquiryTemplateAcceptPolicyDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetAllTemplatePoliciesRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<List<GetInquiryTemplateAcceptPolicyDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        var isTemplateExist = await _inquiriesDbContext.InquiryTemplates
            .AnyAsync(template => template.Id == request.Id, cancellationToken);

        if (!isTemplateExist)
        {
            throw new EntityNotFoundException("Template is not exist");
        }

        return await _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .Where(acceptPolicy => acceptPolicy.InquiryTemplateId == request.Id)
            .AsNoTracking()
            .Select(acceptPolicy => new GetInquiryTemplateAcceptPolicyDto
            {
                Id = acceptPolicy.Id,
                OrderWeight = acceptPolicy.OrderWeight,
                Scope = acceptPolicy.Scope,
                UserFullName = acceptPolicy.User == null
                    ? null : $"{acceptPolicy.User.LastName} {acceptPolicy.User.FirstName}",
                SubstituteScope = acceptPolicy.SubstituteScope,
                SubstituteUserFullName = acceptPolicy.SubstituteUser == null
                    ? null : $"{acceptPolicy.SubstituteUser.LastName} {acceptPolicy.SubstituteUser.FirstName}",
            }).ToListAsync(cancellationToken);
    }

    internal sealed record Command(int Id) : ICommand<List<GetInquiryTemplateAcceptPolicyDto>>, ITransactionalRequest;
}
