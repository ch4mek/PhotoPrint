using Modsen.Portal.Booking.Requests.Dto;

namespace Modsen.Portal.Booking.Requests.Workplaces.Requests.RequestDto;

internal class ReservationDto
{
    public DateTime Date { get; set; }
    public IReadOnlyCollection<UserWorkplaceDto> UserWorkplaces { get; set; } = [];
}
