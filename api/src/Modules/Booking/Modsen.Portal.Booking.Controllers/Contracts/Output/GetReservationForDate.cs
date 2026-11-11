using Modsen.Portal.Booking.Requests.Workplaces.Requests.RequestDto;

namespace Modsen.Portal.Booking.Controllers.Contracts.Output;

public class GetReservationForDate
{
    public DateTime Date { get; set; }
    public IReadOnlyCollection<UserWorkplace> UserWorkplaces { get; set; } = [];

    internal static GetReservationForDate? Of(ReservationDto? source)
    {
        return source == null
            ? null
            : new GetReservationForDate
        {
            UserWorkplaces = source.UserWorkplaces.Select(x => UserWorkplace.Of(x)!).ToArray(),
            Date = source.Date
        };
    }
}
