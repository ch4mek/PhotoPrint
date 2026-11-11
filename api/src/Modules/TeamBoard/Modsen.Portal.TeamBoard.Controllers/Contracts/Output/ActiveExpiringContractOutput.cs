namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record ActiveExpiringContractOutput(
    int Id,
    int UserId,
    string UserFullName,
    DateOnly? ContractTo
);
