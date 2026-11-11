using System.Text.Json.Serialization;

namespace Modsen.Portal.Booking.Controllers.Contracts.Input;

public class GetReservationsForPeriodInput
{
    [JsonRequired]
    public DateTime FromDate { get; set; }

    [JsonRequired]
    public DateTime ToDate { get; set; }
    public int? OfficeId { get; set; }
    public int? WorkspaceId { get; set; }
}
