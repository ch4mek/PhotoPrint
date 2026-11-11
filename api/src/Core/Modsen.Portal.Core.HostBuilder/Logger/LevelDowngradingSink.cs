using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Modsen.Portal.Core.HostBuilder.Logger;

public sealed class LevelDowngradingSink : ILogEventSink, IDisposable
{
    private readonly ILogger _logger;
    private readonly LogEventLevel _targetLevel;

    public LevelDowngradingSink(ILogger logger, LogEventLevel targetLevel = LogEventLevel.Information)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _targetLevel = targetLevel;
    }

    public void Emit(LogEvent? logEvent)
    {
        if (logEvent is null)
        {
            return;
        }

        if (logEvent.Exception is SecurityTokenExpiredException && logEvent.Level > _targetLevel)
        {
            var downgraded = new LogEvent(
                logEvent.Timestamp,
                _targetLevel,
                logEvent.Exception,
                logEvent.MessageTemplate,
                logEvent.Properties.Select(kv => new LogEventProperty(kv.Key, kv.Value)));
            _logger.Write(downgraded);
        }
        else
        {
            _logger.Write(logEvent);
        }
    }

    public void Dispose()
    {
        (_logger as IDisposable)?.Dispose();
    }
}

