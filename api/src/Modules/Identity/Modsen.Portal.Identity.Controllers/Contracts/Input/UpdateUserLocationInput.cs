using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.Controllers.Contracts.Input;

public record UpdateUserLocationInput(
    int UserId,
    LocationType Location);
