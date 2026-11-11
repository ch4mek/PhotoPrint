using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record UserContractOutput(
    int Id,
    int UserId,
    DateOnly ContractFrom,
    DateOnly? ContractTo,
    DateOnly? FireDate,
    FireType? FireType,
    string? FireReason,
    string? FireDescription
    );
