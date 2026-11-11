using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

internal class RegisteredLocationDto
{
    public int RegisteredLocationId { get; set; }
    public string? RegisteredLocationName { get; set; }
    public string? CurrentLocation { get; set; }
    public LocationType? Location { get; set; }
}
