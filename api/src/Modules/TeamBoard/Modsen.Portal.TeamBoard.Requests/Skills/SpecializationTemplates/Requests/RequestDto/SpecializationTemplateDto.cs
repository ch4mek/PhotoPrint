namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

internal class SpecializationTemplateDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public bool CanBeUpdated { get; set; }
}
