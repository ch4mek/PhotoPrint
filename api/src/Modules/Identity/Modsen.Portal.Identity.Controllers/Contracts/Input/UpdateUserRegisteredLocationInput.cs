using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public record UpdateUserRegisteredLocationInput(
    int RegisteredLocationId,
    string? CurrentLocation,
    LocationType Location) {
}
