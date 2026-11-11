namespace Modsen.Portal.TeamBoard.Contract.Contract.Output;

public sealed record UserWithRuNameOutput(
    int Id,
    string? FirstName,
    string? LastName);
