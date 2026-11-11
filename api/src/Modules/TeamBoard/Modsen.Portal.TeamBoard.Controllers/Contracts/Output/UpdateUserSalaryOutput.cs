using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record UpdateUserSalaryOutput(
    int Id,
    int UserId,
    DateOnly Date,
    decimal SalaryUsd)
{
    public static UpdateUserSalaryOutput Of(SalaryDetailsDto source)
    {
        return new UpdateUserSalaryOutput(
            Id: source.Id,
            UserId: source.UserId,
            Date: source.SalaryFrom,
            SalaryUsd: source.SalaryUsd);
    }
}
