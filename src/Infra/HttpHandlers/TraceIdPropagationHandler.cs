using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.HttpHandlers;

public class TraceIdPropagationHandler : DelegatingHandler
{
    private readonly ILogger<TraceIdPropagationHandler> _logger;

    public TraceIdPropagationHandler(ILogger<TraceIdPropagationHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // O .NET automaticamente propaga W3C Trace Context via header 'traceparent'
        // Mas vamos adicionar log para debug e observabilidade
        var currentActivity = Activity.Current;
        if (currentActivity != null && currentActivity.Id != null)
        {
            _logger.LogDebug("[TRACE PROPAGATION] Propagando W3C Trace Context {activityId} para {method} {url}", 
                currentActivity.Id, request.Method, request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}