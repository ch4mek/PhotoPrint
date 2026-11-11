namespace Modsen.Portal.Booking.Controllers.Contracts.Input;

public class CreateBookingBaseInput
{
    public const string CreateSelf = "CreateSelf";
    public const string CreateForUser = "CreateForUser";
    public const string CreateBlock = "CreateBlock";
    public const string CreateForGuest = "CreateForGuest";

    public int WorkplaceId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int WeeksInInterval { get; set; }
    public required string RequestType { get; set; }
    public int? UserId { get; set; }
    public string? GuestName { get; set; }
}
