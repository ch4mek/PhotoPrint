using Modsen.Portal.Booking.Requests.Dto;

namespace Modsen.Portal.Booking.Controllers.Contracts.Output;

public class UserWorkplace
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

    internal static UserWorkplace? Of(UserWorkplaceDto? source)
    {
        return source == null
            ? null
            : new UserWorkplace
            {
                BookingId = source.BookingId,
                WorkplaceNumber = source.WorkplaceNumber,
                WorkplaceId = source.WorkplaceId,
                WorkplaceOfficeId = source.WorkplaceOfficeId,
                UserId = source.UserId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                FirstNameRu = source.FirstNameRu,
                LastNameRu = source.LastNameRu,
                GuestName = source.GuestName,
                BookedBy = source.BookedBy,
                IsForGuest = source.IsForGuest,
                DepartmentId = source.DepartmentId,
                IsBlocked = source.IsBlocked,
            };
    }
}
