namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core.CoreDto;

public sealed class CalendarExclusionCoreDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly? NewDate { get; set; }
    public string? DateType { get; set; }
    public int CountryId { get; set; }
}
