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
        // Logando a requisição
        _logger.LogInformation("[Request] {method} {url}", request.Method, request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);
        timeSpent.Stop();

        // Logando a resposta
        _logger.LogInformation("[Response] {method} {url} responded {statusCode} in {time}ms", request.Method, request.RequestUri, (int)response.StatusCode, timeSpent.ElapsedMilliseconds);

        return response;
    }
}

