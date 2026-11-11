namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record InternshipSpecializationInput
{
    public int? Id { get; init; }
    public string? SeniorityLevelId { get; init; }
    public int SpecializationTemplateId { get; init; }
    public IReadOnlyCollection<HardSkillSeniorityInput> HardSkills { get; init; } = [];
}
