using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class CalendarExclusionOutput
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly Date { get; set; }

    public DateOnly? NewDate { get; set; }
    public DateType DateType { get; set; }
    public required int CountryId { get; set; }

    public static CalendarExclusionOutput Of(CalendarExclusionDto dto)
    {
        return new CalendarExclusionOutput
        {
            Id = dto.Id,
            Name = dto.Name,
            Date = dto.Date,
            NewDate = dto.NewDate,
            DateType = dto.DateType,
            CountryId = dto.CountryId
        };
    }
}
