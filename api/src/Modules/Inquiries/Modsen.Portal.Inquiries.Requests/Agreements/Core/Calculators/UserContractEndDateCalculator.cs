using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;

internal interface IUserContractEndDateCalculator
{
    Task<DateOnly?> Calculate(int userId, CancellationToken cancellationToken);
}

internal class UserContractEndDateCalculator : IUserContractEndDateCalculator
{
    private readonly ITeamBoardContract _teamBoardContract;

    public UserContractEndDateCalculator(ITeamBoardContract teamBoardContract)
    {
        _teamBoardContract = teamBoardContract;
    }

    public async Task<DateOnly?> Calculate(int userId, CancellationToken cancellationToken)
    {
        var userContractDates = await _teamBoardContract.GetUserContractDates(userId, cancellationToken);
        var contractEndDate = userContractDates?.FireDate;
        if (contractEndDate == null)
        {
            return null;
        }

        return contractEndDate.Value;
    }
}
