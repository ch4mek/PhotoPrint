namespace Modsen.Portal.TeamBoard.Requests.Payload.Requests.RequestDto;

public record PayloadUserFteRequestDto(
    int UserId,
    int Year,
    int Month,
    decimal Fte,
    decimal PlannedWorkloadInHours);
