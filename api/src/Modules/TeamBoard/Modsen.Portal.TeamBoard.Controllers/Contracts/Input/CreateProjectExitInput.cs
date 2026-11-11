namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public record CreateProjectExitInput(
    int UserId,
    int ProjectId,
    int ClientId,
    DateOnly ClosingDate,
    string ClosingReasonName);
