namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record SpecializationTemplateOutput(
    int Id,
    string Name,
    int CreatedById,
    bool CanBeUpdated);
