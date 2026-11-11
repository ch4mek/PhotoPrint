using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record CreateUserContractInput(
    int UserId,
    DateOnly ContractFrom,
    DateOnly ContractTo,
    DateOnly? FireDate,
    FireType? FireType,
    string? FireReason,
    string? FireDescription);
