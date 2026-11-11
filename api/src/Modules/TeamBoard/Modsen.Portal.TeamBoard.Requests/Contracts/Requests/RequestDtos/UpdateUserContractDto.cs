using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

internal record UpdateUserContractDto(
    int Id,
    int UserId,
    DateOnly ContractFrom,
    DateOnly ContractTo,
    DateOnly? FireDate,
    FireType? FireType,
    string? FireReason,
    string? FireDescription);
