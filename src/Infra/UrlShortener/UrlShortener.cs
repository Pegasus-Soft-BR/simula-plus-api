using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockExams.Infra.Sms;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MockExams.Infra.UrlShortener;

public class UrlShortener : IUrlShortener
{
    private readonly HttpClient _httpClient;
    private UrlShortenerSettings _settings { get; set; }
    private readonly ILogger<UrlShortener> _logger;

    public UrlShortener(IOptions<UrlShortenerSettings> settings, IHttpClientFactory httpClientFactory, ILogger<UrlShortener> logger)
    {
        _settings = settings.Value;
        _httpClient = _httpClient = httpClientFactory.CreateClient("DefaultClient");
        _logger = logger;
    }

    public string GetShortUrl(string longUrl)
    {
        try
        {
            _logger.LogDebug("[URL SHORTENER] Iniciando.");

            if (!_settings.IsActive)
            {
                _logger.LogDebug("[URL SHORTENER] Não vou executar porque estou desabilitado no settings.");
                return longUrl;
            }

            var url = "https://url-shortener-service.p.rapidapi.com/shorten";
            var apiKey = _settings.ApiKey;
            var host = "url-shortener-service.p.rapidapi.com";
            var targetUrl = longUrl;

            var content = new StringContent($"url={Uri.EscapeDataString(targetUrl)}", Encoding.UTF8, "application/x-www-form-urlencoded");

            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", host);

            var response = _httpClient.PostAsync(url, content).Result;

            response.EnsureSuccessStatusCode();

            var responseText = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<UrlShortenerResult>(responseText);
            return result.result_url;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[URL SHORTENER] Ocorreu um erro ao encurtar a URL: {message}", ex.Message);
            return longUrl;
        }

    }
}

public class UrlShortenerResult {
    public string result_url { get; set; }
}
