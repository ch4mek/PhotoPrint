namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public record UpdateTeamBoardUserCommentInput(
    int UserId,
    string? Comment);
