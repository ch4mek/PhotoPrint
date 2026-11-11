namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public record UpdateUserProjectAssignmentInput(int Id,
    int UserId,
    int ProjectId,
    bool IsTimeOnProjectBillable,
    decimal Fte,
    string UserOnProjectSeniorityLevelId,
    DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo);
