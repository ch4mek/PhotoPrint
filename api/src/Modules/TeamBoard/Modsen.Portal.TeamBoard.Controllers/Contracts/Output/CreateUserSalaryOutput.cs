using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record CreateUserSalaryOutput(
    int Id,
    int UserId,
    DateOnly Date,
    decimal SalaryUsd)
{
    public static CreateUserSalaryOutput Of(SalaryDetailsDto source)
    {
        return new CreateUserSalaryOutput(
            Id: source.Id,
            UserId: source.UserId,
            Date: source.SalaryFrom,
            SalaryUsd: source.SalaryUsd);
    }
}
