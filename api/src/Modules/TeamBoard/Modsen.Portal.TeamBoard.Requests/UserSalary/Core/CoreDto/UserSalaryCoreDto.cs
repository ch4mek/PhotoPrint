namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;

internal record UserSalaryCoreDto(
    int Id,
    int UserId,
    DateOnly SalaryFrom,
    decimal SalaryUsd,
    int? CreatedById,
    DateTime CreatedAtUtc);
