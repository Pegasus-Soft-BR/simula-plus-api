using Helper;
using Infra.PegasusApi.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Infra.PegasusApi;

public class PegasusApiClient : IPegasusApiClient
{
    private readonly HttpClient _httpClient;
    private readonly PegasusApiSettings _settings;
    private readonly ILogger<PegasusApiClient> _logger;

    public PegasusApiClient(IHttpClientFactory httpClientFactory, IOptions<PegasusApiSettings> settings, ILogger<PegasusApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("DefaultClient");
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ContactUsResponse> SendContactUsAsync(ContactUsRequest request)
    {
        if (!_settings.IsActive)
        {
            _logger.LogDebug("[PEGASUS API] Não vou executar porque estou desabilitado no settings.");
            throw new InvalidOperationException("O serviço PegasusApi está desativado no appsettings.");
        }

        var url = $"{_settings.BaseUrl}/api/contact-us";
        _logger.LogDebug("[PEGASUS API] Enviando mensagem de contato para {url}", url);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        var response = await _httpClient.SendAsync(httpRequest);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("[PEGASUS API] Erro HTTP {statusCode}: {reasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);

            // Se houver erro HTTP, tenta deserializar a resposta de erro
            try
            {
                var errorResponse = JsonHelper.FromJson<ContactUsResponse>(content);
                _logger.LogWarning("[PEGASUS API] Resposta de erro da API: {errorMessages}", string.Join(", ", errorResponse.ErrorMessages));
                return errorResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PEGASUS API] Erro ao deserializar resposta de erro");
                // Se não conseguir deserializar, retorna erro genérico
                return new ContactUsResponse
                {
                    Success = false,
                    ErrorMessages = new List<string> { $"Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}" }
                };
            }
        }

        var successResponse = JsonHelper.FromJson<ContactUsResponse>(content);
        _logger.LogDebug("[PEGASUS API] Mensagem enviada com sucesso: {successMessage}", successResponse.SuccessMessage);
        return successResponse;
    }
}