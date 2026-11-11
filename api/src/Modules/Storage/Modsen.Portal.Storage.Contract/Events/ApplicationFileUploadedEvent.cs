using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Storage.Contract.Events;

public record ApplicationFileUploadedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required string FileId { get; init; }
    public required ApplicationFilePurpose FilePurpose { get; init; }
    public required string ContentType { get; init; }
}

internal sealed class ApplicationFileUploadedExchangeDefinition : ExchangeDefinitionBase<ApplicationFileUploadedEvent>
{
    public override string ExchangeName => "application-file-uploaded-exchange";
}
