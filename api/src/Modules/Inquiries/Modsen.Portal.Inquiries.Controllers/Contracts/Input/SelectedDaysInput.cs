namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

public record SelectedDaysInput
{
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
}
