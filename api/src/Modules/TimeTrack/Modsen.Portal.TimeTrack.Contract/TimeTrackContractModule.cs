using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.TimeTrack.Contract;

public sealed class TimeTrackContractModule : RegistrationModule
{
    public override IEnumerable<IExchangeDefinitionBase> GetExchangeDefinitions()
    {
        yield return new ClientCreatedExchangeDefinition();
        yield return new ClientUpdatedExchangeDefinition();
        yield return new ProjectCreatedExchangeDefinition();
        yield return new ProjectDeactivatedExchangeDefinition();
        yield return new ProjectUpdatedExchangeDefinition();
        yield return new ProjectUserCreatedExchangeDefinition();
        yield return new ProjectUserUpdatedExchangeDefinition();
    }
}
