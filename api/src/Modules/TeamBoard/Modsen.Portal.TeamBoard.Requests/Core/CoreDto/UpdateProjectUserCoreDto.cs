namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal record UpdateProjectUserCoreDto(
    int Id,
    int UserId,
    int ProjectId,
    bool IsTimeOnProjectBillable,
    decimal Fte,
    string? SeniorityLevelId,
    DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo);
