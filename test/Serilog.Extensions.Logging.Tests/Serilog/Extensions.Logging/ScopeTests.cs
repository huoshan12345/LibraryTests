using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Extensions.Logging;

public class ScopeTests
{
    [Fact]
    public async Task PropertyOverride_Scope()
    {
        var listener = new LogEventListener();

        var serilog = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(listener)
            .CreateLogger();

        var provider = new ServiceCollection()
            .AddLogging(m => m.AddSerilog(serilog, dispose: true))
            .BuildServiceProvider();

        var logger = provider.GetRequiredService<ILogger<ScopeTests>>();

        using (logger.BeginScope(new Dictionary<string, object?> { { "Value", 1 } }))
        using (logger.BeginScope(new Dictionary<string, object?> { { "Value", 2 } }))
        {
            logger.LogInformation("Value: {Value}", 3);
        }

        Assert.True(await listener.WaitAsync(1, TimeSpan.FromSeconds(3)));

        var logEvent = listener.Events.First();
        var value = (logEvent.Properties["Value"] as ScalarValue)?.Value;

        // since Serilog.Extensions.Logging 9.0.1
        Assert.Equal(2, value);

        // before Serilog.Extensions.Logging 9.0.1
        // Assert.Equal(3, value);
    }

    [Fact]
    public async Task PropertyOverride_LogContext()
    {
        var listener = new LogEventListener();

        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(listener)
            .CreateLogger();

        using (LogContext.PushProperty("Value", 1))
        using (LogContext.PushProperty("Value", 2))
        {
            logger.Information("Value: {Value}", 3);
        }

        Assert.True(await listener.WaitAsync(1, TimeSpan.FromSeconds(3)));

        var logEvent = listener.Events.First();
        Assert.True(logEvent.Properties.TryGetValue("Value", out var value));
        Assert.Equal(3, (value as ScalarValue)?.Value);
    }
}