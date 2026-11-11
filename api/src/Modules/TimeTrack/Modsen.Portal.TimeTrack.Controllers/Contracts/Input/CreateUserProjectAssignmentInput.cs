namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public record CreateUserProjectAssignmentInput(DateOnly UserOnProjectFrom, DateOnly? UserOnProjectTo,
    bool IsTimeOnProjectBillable, decimal Fte, string UserOnProjectSeniorityLevelId);
