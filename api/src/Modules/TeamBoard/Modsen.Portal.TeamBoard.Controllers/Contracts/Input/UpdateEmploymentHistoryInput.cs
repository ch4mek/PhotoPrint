namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

public record UpdateEmploymentHistoryInput(
    int UserId,
    DateOnly? ProfessionalActivityStartDate,
    DateOnly? FireDate,
    DateOnly? NdaStartDate = null,
    DateOnly? NdaEndDate = null);

