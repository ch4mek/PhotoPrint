namespace Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;

internal class BookingEntryCoreDto
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public bool IsForGuest { get; set; }
    public string? GuestName { get; set; }
    public List<DateTime> DateList { get; set; } = [];
    public DateTime From { get; set; }
    public bool IsBlock { get; set; }
    public int WorkplaceId { get; set; }
    public int WorkplaceOfficeId { get; set; }
    public int WorkplaceNumber { get; set; }
    public int? WorkspaceId { get; set; }
    public int WeeksInInterval { get; set; }
}
