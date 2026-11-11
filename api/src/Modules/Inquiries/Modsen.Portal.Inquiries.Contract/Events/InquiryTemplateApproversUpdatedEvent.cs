using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Inquiries.Contract.Events;

public class InquiryTemplateApproversUpdatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required IReadOnlyCollection<int> ApproversIds { get; init; }
}

internal sealed class InquiryTemplateApproversUpdatedExchangeDefinition : ExchangeDefinitionBase<InquiryTemplateApproversUpdatedEvent>
{
    public override string ExchangeName => "inquiry-template-approvers-updated-exchange";
}
