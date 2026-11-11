using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests.RequestDto;

public class CalendarExclusionDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly? NewDate { get; set; }
    public DateType DateType { get; set; }
    public int CountryId { get; set; }
}
