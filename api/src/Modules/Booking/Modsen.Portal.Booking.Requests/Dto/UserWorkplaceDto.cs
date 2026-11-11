namespace Modsen.Portal.Booking.Requests.Dto;

internal class UserWorkplaceDto
{
    public int BookingId { get; set; }
    public int WorkplaceNumber { get; set; }
    public int WorkplaceId { get; set; }
    public int WorkplaceOfficeId { get; set; }
    public int? UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? GuestName { get; set; }
    public string? BookedBy { get; set; }
    public bool IsForGuest { get; set; }
    public string? DepartmentId { get; set; }
    public bool IsBlocked { get; set; }
}
