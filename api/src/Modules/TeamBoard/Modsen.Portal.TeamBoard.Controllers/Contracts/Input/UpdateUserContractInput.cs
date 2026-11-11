using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record UpdateUserContractInput(
    int Id,
    DateOnly ContractFrom,
    DateOnly ContractTo,
    DateOnly? FireDate,
    FireType? FireType,
    string? FireReason,
    string? FireDescription);
