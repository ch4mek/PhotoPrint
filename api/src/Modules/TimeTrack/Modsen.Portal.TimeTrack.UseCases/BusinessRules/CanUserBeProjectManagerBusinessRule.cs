using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.TimeTrack.UseCases.BusinessRules;

internal interface ICanUserBeProjectManagerBusinessRule : IBusinessRule<int>;

internal class CanUserBeProjectManagerBusinessRule : ICanUserBeProjectManagerBusinessRule
{
    private readonly IIdentityContract _identityContract;

    public CanUserBeProjectManagerBusinessRule(IIdentityContract identityContract)
    {
        _identityContract = identityContract;
    }

    public Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        return _identityContract.CanUserBeProjectManagerByDepartment(userId, cancellationToken);
    }
}
