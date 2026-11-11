using Modsen.Portal.Storage.Contract.Events;

namespace Modsen.Portal.Storage.Requests.Jobs;

internal interface IApplicationFileVariantsResizeJobDispatcher
{
    void Dispatch(ApplicationFileUploadedEvent @event);
}


