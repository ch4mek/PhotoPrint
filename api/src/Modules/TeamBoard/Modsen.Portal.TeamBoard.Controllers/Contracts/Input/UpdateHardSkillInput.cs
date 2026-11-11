namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public record UpdateHardSkillInput(
    int Id,
    string Name,
    string HardSkillGroupId);
