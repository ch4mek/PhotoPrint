using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;

internal interface IInquiryTemplateValidator
{
    Task Validate(InquiryTemplateEntity template, CancellationToken cancellationToken);
}

internal class InquiryTemplateValidator : IInquiryTemplateValidator
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public InquiryTemplateValidator(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task Validate(InquiryTemplateEntity template, CancellationToken cancellationToken)
    {
        var exist = await _inquiriesDbContext.InquiryTemplates
            .AnyAsync(x=>x.IsActive && x.Name == template.Name && x.Id != template.Id, cancellationToken);
        if (exist)
        {
            throw new ValidationFailedException("Absence with the same name already exists");
        }
    }
}
