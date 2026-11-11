using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Identity.Contract;

public sealed class IdentityContractModule : RegistrationModule
{
    public override IEnumerable<IExchangeDefinitionBase> GetExchangeDefinitions()
    {
        yield return new DeleteFileExchangeDefinition();
        yield return new DepartmentSyncExchangeDefinition();
        yield return new ResourceGroupCreatedExchangeDefinition();
        yield return new ResourceGroupSyncExchangeDefinition();
        yield return new ResourceGroupUpdatedExchangeDefinition();
        yield return new UserActivatedExchangeDefinition();
        yield return new UserCreatedExchangeDefinition();
        yield return new UserDeactivatedExchangeDefinition();
        yield return new UserLocationUpdatedExchangeDefinition();
        yield return new UserRegisteredLocationUpdatedExchangeDefinition();
        yield return new UserSyncExchangeDefinition();
        yield return new UserUpdatedExchangeDefinition();
    }
}
