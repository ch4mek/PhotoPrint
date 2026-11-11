using Modsen.Portal.Core;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Contracts.Output;

public class RegisteredLocationOutput
{
    public int RegisteredLocationId { get; set; }
    public string? RegisteredLocationName { get; set; }
    public string? CurrentLocation { get; set; }
    public LocationType? Location { get; set; }


    internal static RegisteredLocationOutput Of(RegisteredLocationDto source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new RegisteredLocationOutput
        {
            RegisteredLocationId = source.RegisteredLocationId,
            RegisteredLocationName = source.RegisteredLocationName,
            CurrentLocation = source.CurrentLocation,
            Location = source.Location
        };
    }
}
