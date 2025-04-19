using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Api.Configuration;

public class SerilogTraceIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Activity.Current?.TraceId.ToString();
        if (!string.IsNullOrWhiteSpace(traceId))
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", traceId));
        }
    }
}
