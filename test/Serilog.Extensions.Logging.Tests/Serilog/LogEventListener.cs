using Serilog.Core;
using Serilog.Events;

namespace Serilog;

public class LogEventListener : ILogEventSink
{
    private readonly SemaphoreSlim _semaphore = new(0);
    public List<LogEvent> Events { get; } = [];

    public virtual void Emit(LogEvent logEvent)
    {
        Events.Add(logEvent);
        _semaphore.Release();
    }

    public async Task<bool> WaitAsync(int count, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        for (var i = 0; i < count; ++i)
        {
            var flag = await _semaphore.WaitAsync(timeout, cancellationToken);
            if (flag == false)
                return false; // timeout
        }
        return true;
    }
}
