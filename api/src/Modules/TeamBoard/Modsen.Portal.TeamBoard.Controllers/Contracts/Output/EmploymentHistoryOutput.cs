namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

public record EmploymentHistoryOutput(
    DateOnly? ProfessionalActivityStartDate,
    DateOnly? FirstProjectStartDate,
    DateOnly? ContractEndDate,
    DateOnly? HireDate,
    DateOnly? FireDate,
    DateOnly? NdaStartDate,
    DateOnly? NdaEndDate,
    bool ShouldDisplayNda
);
