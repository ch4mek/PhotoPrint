namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

internal record ActiveExpiringContractDto(
    int Id,
    int UserId,
    string UserFullName,
    DateOnly? ContractTo,
    string? UserPosition);
