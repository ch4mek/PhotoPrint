using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record CreateUserSnapshotInput(
    int UserId,
    DateOnly SnapshotDate,
    IReadOnlyCollection<CreateUserSpecializationDto> UserSpecializations
);
