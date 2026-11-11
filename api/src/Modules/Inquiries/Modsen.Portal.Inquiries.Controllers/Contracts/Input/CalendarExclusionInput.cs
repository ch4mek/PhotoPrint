using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal class CalendarExclusionInput
{
    public string? Name { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly? NewDate { get; set; }
    public DateType DateType { get; set; }
    public required int CountryId { get; set; }
}
