using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public record DeleteFileEvent(string FileId, string EventId, FileSource? FileSource = null) : IIntegrationEvent;

internal sealed class DeleteFileExchangeDefinition : ExchangeDefinitionBase<DeleteFileEvent>
{
    public override string ExchangeName => "delete-file-exchange";
}
