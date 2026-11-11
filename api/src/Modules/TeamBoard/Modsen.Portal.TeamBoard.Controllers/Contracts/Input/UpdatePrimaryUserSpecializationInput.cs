namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public record UpdatePrimaryUserSpecializationInput(
    int UserId,
    int NewSpecializationTemplateId);
