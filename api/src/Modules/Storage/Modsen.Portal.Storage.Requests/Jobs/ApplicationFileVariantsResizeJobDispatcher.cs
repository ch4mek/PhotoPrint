using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.Contract.Events;

namespace Modsen.Portal.Storage.Requests.Jobs;

internal sealed class ApplicationFileVariantsResizeJobDispatcher(
    IBackgroundJobManager backgroundJobManager,
    IServiceProvider serviceProvider,
    ILogger<ApplicationFileVariantsResizeJobDispatcher> logger)
    : IApplicationFileVariantsResizeJobDispatcher
{
    public void Dispatch(ApplicationFileUploadedEvent @event)
    {
        backgroundJobManager.Schedule("default", () => Execute(@event));
    }

    [AutomaticRetry(
        Attempts = 3,
        DelaysInSeconds = [30, 120, 600],
        OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task Execute(ApplicationFileUploadedEvent @event)
    {
        using var scope = serviceProvider.CreateScope();

        try
        {
            await scope.ServiceProvider
                .GetRequiredService<ApplicationFileVariantsResizeJob>()
                .Execute(@event, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "ApplicationFileVariantsResizeJob failed. FileId={FileId}, EventId={EventId}",
                @event.FileId, @event.EventId);
            throw;
        }
    }
}
