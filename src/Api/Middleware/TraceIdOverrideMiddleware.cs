using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Api.Middleware;

public class TraceIdOverrideMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TraceIdOverrideMiddleware> _logger;

    public TraceIdOverrideMiddleware(RequestDelegate next, ILogger<TraceIdOverrideMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Trace-Id", out var traceIdValue))
        {
            var traceId = traceIdValue.ToString();

            if (IsValidTraceId(traceId))
            {
                string parentId = $"00-{traceId}-0000000000000000-01"; // W3C traceparent

                var incoming = new Activity("IncomingRequest");
                incoming.SetIdFormat(ActivityIdFormat.W3C);
                incoming.SetParentId(parentId);
                incoming.Start();

                Activity.Current = incoming;
            }
            else
            {
                _logger.LogWarning("X-Trace-Id recebido com formato inválido: {TraceId}", traceId);
            }
        }

        await _next(context);
    }

    private static bool IsValidTraceId(string traceId) =>
        !string.IsNullOrWhiteSpace(traceId)
        && traceId.Length == 32
        && Regex.IsMatch(traceId, "^[a-fA-F0-9]{32}$");
}
