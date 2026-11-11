using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;

internal interface IIsEnoughAvailableDaysForInquiryBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int inquiryTemplateId, int userId, DateOnly dateFrom,
        DateOnly dateTo, CancellationToken cancellationToken);
}

internal class IsEnoughAvailableDaysForInquiryBusinessRule : IIsEnoughAvailableDaysForInquiryBusinessRule
{
    private readonly IInquiriesDbContext _inquiryDbContext;
    private readonly ITotalAccountantDaysCalculator _totalAccountantDaysCalculator;
    private readonly IUserAgreementInquiryAvailableDaysCalculator _availableDaysCalculator;
    private readonly IInquiryDatesComposer _inquiryDatesComposer;

    public IsEnoughAvailableDaysForInquiryBusinessRule(IInquiriesDbContext inquiryDbContext,
        ITotalAccountantDaysCalculator totalAccountantDaysCalculator,
        IUserAgreementInquiryAvailableDaysCalculator availableDaysCalculator,
        IInquiryDatesComposer inquiryDatesComposer)
    {
        _inquiryDbContext = inquiryDbContext;
        _totalAccountantDaysCalculator = totalAccountantDaysCalculator;
        _availableDaysCalculator = availableDaysCalculator;
        _inquiryDatesComposer = inquiryDatesComposer;
    }

    public async Task<bool> IsCorrespondsToBusiness(int inquiryTemplateId, int userId,
        DateOnly dateFrom, DateOnly dateTo, CancellationToken cancellationToken)
    {
        var inquiryTemplate = await _inquiryDbContext.InquiryTemplates
            .Where(x => x.Id == inquiryTemplateId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry Template with id {inquiryTemplateId} does not exist");

        if (inquiryTemplate.InquiryType != InquiryType.SickDay)
        {
            return true;
        }

        var availableDays = await _availableDaysCalculator.CalculateAsync(userId, inquiryTemplateId,
            cancellationToken);

        var countryId = await _inquiryDbContext.InquiriesUsers
            .Where(x => x.UserId == userId)
            .Select(x => x.RegisteredLocationId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.RegisteredLocationNotFound);

        var inquiryDates = await  _inquiryDatesComposer.Compose(new InquiryDatesComposerRequest
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            RequestedDays = dateTo.DayNumber - dateFrom.DayNumber + 1,
            CountryId = countryId
        }, cancellationToken);

        var requestedDaysCount = _totalAccountantDaysCalculator.Calculate(inquiryTemplate.IncludeHolidays,
            inquiryTemplate.IncludeWeekends, inquiryDates.Select(x => (x.DateType, x.Date)).ToList());

        return requestedDaysCount.TotalAccoundantDays <= availableDays;
    }
}
