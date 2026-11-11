using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record UpdateUserSpecializationsInput(
    int SnapshotId,
    int UserId,
    IReadOnlyCollection<CreateUserSpecializationDto> UserSpecializations
);
