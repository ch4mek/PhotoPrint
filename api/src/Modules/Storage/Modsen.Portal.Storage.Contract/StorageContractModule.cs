using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Storage.Contract.Events;

namespace Modsen.Portal.Storage.Contract;

public sealed class StorageContractModule : RegistrationModule
{
    public override IEnumerable<IExchangeDefinitionBase> GetExchangeDefinitions()
    {
        yield return new ApplicationFileUploadedExchangeDefinition();
    }
}
