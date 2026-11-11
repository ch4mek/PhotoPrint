using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Notification.Contract.Events;

namespace Modsen.Portal.Notification.Contract;

public sealed class NotificationContractModule : RegistrationModule
{
    public override IEnumerable<IExchangeDefinitionBase> GetExchangeDefinitions()
    {
        yield return new UserDeactivatedExchangeDefinition();
        yield return new NotificationCreatedExchangeDefinition();
    }
}
