using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.TeamBoard.Contract.Events
{
    public class ContractCreatedEvent : IIntegrationEvent
    {
        public required string EventId { get; init; }

        public int ContractId { get; set; }
        public int UserId { get; set; }
        public DateOnly ContractFrom { get; set; }
        public DateOnly ContractTo { get; set; }
        public DateOnly? FireDate { get; set; }
    }

    internal sealed class ContractCreatedExchangeDefinition : ExchangeDefinitionBase<ContractCreatedEvent>
    {
        public override string ExchangeName => "contract-created-exchange";
    }

}
