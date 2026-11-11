namespace Modsen.Portal.Booking.Controllers.Contracts.Input;

public class GetBookingsInput
{
    public int? UserId { get; set; }
    public int? OfficeId { get; set; }
    public int? WorkspaceId { get; set; }
    public bool IncludeEmployee { get; set; }
}
