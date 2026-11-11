namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record HardSkillByIdOutput(
    int Id,
    string Name,
    string HardSkillGroupId,
    string HardSkillGroupName);
