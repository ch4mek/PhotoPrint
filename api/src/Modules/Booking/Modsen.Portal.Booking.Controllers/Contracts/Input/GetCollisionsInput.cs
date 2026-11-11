namespace Modsen.Portal.Booking.Controllers.Contracts.Input;

internal class GetCollisionsInput
{
    public int UserId { get; set; }
    public int OfficeId { get; set; }
    public int WorkplaceId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int WeeksInterval { get; set; }
}
