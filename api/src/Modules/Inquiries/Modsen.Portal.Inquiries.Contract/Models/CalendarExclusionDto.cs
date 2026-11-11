using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Contract.Models;

public class CalendarExclusionDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public DateOnly Date { get; init; }
    public DateOnly? NewDate { get; init; }
    public DateType DateType { get; init; }
    public required int CountryId { get; init; }
}
