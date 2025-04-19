using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.HttpHandlers;

public class LoggingHttpMessageHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHttpMessageHandler> _logger;

    public LoggingHttpMessageHandler(ILogger<LoggingHttpMessageHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var timeSpent = Stopwatch.StartNew();

        // Adiciona o TraceId no header da requisição (se existir)
        var traceId = Activity.Current?.TraceId.ToString();
        if (!string.IsNullOrWhiteSpace(traceId))
            request.Headers.TryAddWithoutValidation("X-Trace-Id", traceId);

        _logger.LogInformation("[Request] {method} {url}", request.Method, request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);
        timeSpent.Stop();


        _logger.LogInformation("[Response] {method} {url} responded {statusCode} in {time}ms", request.Method, request.RequestUri, (int)response.StatusCode, timeSpent.ElapsedMilliseconds);

        return response;
    }
}

