using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

internal record SalaryDetailsDto(
    int Id,
    int UserId,
    DateOnly SalaryFrom,
    decimal SalaryUsd)
{
    public static SalaryDetailsDto Of(UserSalaryCoreDto source)
    {
        return new SalaryDetailsDto(
            Id: source.Id,
            UserId: source.UserId,
            SalaryFrom: source.SalaryFrom,
            SalaryUsd: source.SalaryUsd);
    }
}
