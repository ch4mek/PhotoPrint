using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;

internal interface IInquiryDatesComposer
{
    Task<List<UserInquiryDateEntity>> Compose(InquiryDatesComposerRequest request, CancellationToken cancellationToken);
}

internal class InquiryDatesComposer : IInquiryDatesComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public InquiryDatesComposer(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<List<UserInquiryDateEntity>> Compose(InquiryDatesComposerRequest request, CancellationToken cancellationToken)
    {
        if (request.DateFrom > request.DateTo)
        {
            throw new InvalidDataAppException("It is impossible to create a inquiry with 0 or less days");
        }

        var inquiryDates = Enumerable
            .Range(0, request.DateTo.DayNumber - request.DateFrom.DayNumber + 1)
            .Select(i => request.DateFrom.AddDays(i))
            .ToList();

        var calendarExclusions = (await _inquiriesDbContext.CalendarExclusions
            .Where(x => x.CountryId == request.CountryId)
            .Where(x => (x.Date >= request.DateFrom && x.Date <= request.DateTo)
                || (x.NewDate >= request.DateFrom && x.NewDate <= request.DateTo))
            .ToListAsync(cancellationToken))
            .SelectMany(x => x.NewDate.HasValue
                ? new List<Tuple<DateOnly, DateType, int>>{new(x.Date, DateType.DayOff, x.Id), new(x.NewDate.Value, DateType.WorkingDay, x.Id)}
                : new List<Tuple<DateOnly, DateType, int>>{new(x.Date, x.DateType, x.Id)})
            .ToList();

        return inquiryDates.GroupJoin(
            calendarExclusions,
            date => date,
            exclusion => exclusion.Item1,
            (date, matchingExclusions) =>
            {
                var exclusion = matchingExclusions.FirstOrDefault();
                return new UserInquiryDateEntity
                {
                    Date = date,
                    RequestedDays = request.RequestedDays,
                    DateType = exclusion?.Item2 ?? GetDateTypeFromDayOfWeek(date.DayOfWeek),
                    CalendarExclusionId = exclusion?.Item3
                };
            }).ToList();
    }

    private static DateType GetDateTypeFromDayOfWeek(DayOfWeek date)
    {
        return date switch
        {
            DayOfWeek.Saturday => DateType.Saturday,
            DayOfWeek.Sunday => DateType.Sunday,
            _ => DateType.WorkingDay
        };
    }
}

internal class InquiryDatesComposerRequest
{
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public int CountryId { get; set; }
    public int RequestedDays { get; set; }
}
