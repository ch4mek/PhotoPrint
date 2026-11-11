using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Contract.Events;

namespace Modsen.Portal.Inquiries.Contract;

public sealed class InquiryContractModule : RegistrationModule
{
    public override IEnumerable<IExchangeDefinitionBase> GetExchangeDefinitions()
    {
        yield return new InquiryTemplateApproversUpdatedExchangeDefinition();
    }
}
