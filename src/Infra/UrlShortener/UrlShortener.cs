using Microsoft.Extensions.Options;
using MockExams.Infra.Sms;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MockExams.Infra.UrlShortener;

public class UrlShortener : IUrlShortener
{
    private UrlShortenerSettings _settings { get; set; }

    public UrlShortener(IOptions<UrlShortenerSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GetShortUrl(string longUrl)
    {
        try
        {
            Console.WriteLine("[URL SHORTENER] Iniciando.");

            if (!_settings.IsActive)
            {
                // TODO: logar alguma coisa.
                Console.WriteLine("[URL SHORTENER] Não vou executar porque estou desabilitado no settings.");
                return longUrl;
            }

            HttpClient client = new HttpClient();

            var url = "https://url-shortener-service.p.rapidapi.com/shorten";
            var apiKey = _settings.ApiKey;
            var host = "url-shortener-service.p.rapidapi.com";
            var targetUrl = longUrl;

            var content = new StringContent($"url={Uri.EscapeDataString(targetUrl)}", Encoding.UTF8, "application/x-www-form-urlencoded");

            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Host", host);

            var response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseText = response.Content.ReadAsStringAsync().Result;
                var result = JsonSerializer.Deserialize<UrlShortenerResult>(responseText);
                return result.result_url;
            }
            else
            {
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // TODO: logar erro.
            Console.WriteLine("[URL SHORTENER]" + ex.Message);
            return longUrl;
        }

    }
}

public class UrlShortenerResult {
    public string result_url { get; set; }
}
