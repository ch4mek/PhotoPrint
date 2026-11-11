using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class GetInquirySelectedDaysRequest : IQueryHandler<GetInquirySelectedDaysRequest.Query, int>
{
    private readonly IInquiryDatesComposer _inquiryDatesComposer;
    private readonly ITotalAccountantDaysCalculator _totalAccountantDaysCalculator;
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IHasUserAccessToInquiryBusinessRule _hasUserAccessToInquiryBusinessRule;

    public GetInquirySelectedDaysRequest(
        IInquiryDatesComposer inquiryDatesComposer,
        ITotalAccountantDaysCalculator totalAccountantDaysCalculator,
        IInquiriesDbContext inquiriesDbContext,
        IHasUserAccessToInquiryBusinessRule hasUserAccessToInquiryBusinessRule)
    {
        _inquiryDatesComposer = inquiryDatesComposer;
        _totalAccountantDaysCalculator = totalAccountantDaysCalculator;
        _inquiriesDbContext = inquiriesDbContext;
        _hasUserAccessToInquiryBusinessRule = hasUserAccessToInquiryBusinessRule;
    }

    public async Task<int> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.DateFrom > request.DateTo ||
            request.DateFrom.AddYears(1) < request.DateTo)
        {
            throw new InvalidDataAppException("The date range is invalid");
        }

        if(!await _hasUserAccessToInquiryBusinessRule.IsCorrespondsToBusiness(request.UserId,
               AccessOptions.Manager | AccessOptions.Hr, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.UserNotFound);
        }

        var userCountryId = await _inquiriesDbContext.InquiriesUsers
            .Where(user => user.UserId == request.UserId)
            .Select(user => user.RegisteredLocationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userCountryId is null)
        {
            throw new EntityNotFoundException($"Cannot find country for user with ID {request.UserId}");
        }

        var inquiryTemplate = await _inquiriesDbContext.InquiryTemplates
            .FirstOrDefaultAsync(inquiryTemplate => inquiryTemplate.Id == request.InquiryTemplateId,
                cancellationToken);

        if (inquiryTemplate is null)
        {
            throw new EntityNotFoundException($"Cannot find inquiry template with ID {request.InquiryTemplateId}");
        }

        var inquiryDateEntities = await _inquiryDatesComposer.Compose(
            new InquiryDatesComposerRequest
            {
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                CountryId = userCountryId.Value,
                RequestedDays = 1
            }, cancellationToken);

        var totalAccountantDaysCoreDto = _totalAccountantDaysCalculator.Calculate(inquiryTemplate.IncludeHolidays,
            inquiryTemplate.IncludeWeekends,
            inquiryDays: inquiryDateEntities.Select(inquiryDate => (inquiryDate.DateType, inquiryDate.Date)).ToList());

        var totalAccountantDays = totalAccountantDaysCoreDto.TotalAccoundantDays;

        return (int)totalAccountantDays;
    }

    internal sealed record Query(int InquiryTemplateId, int UserId, DateOnly DateFrom, DateOnly DateTo) : IQuery<int>;
}
