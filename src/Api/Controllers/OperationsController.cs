using Domain.DTOs;
using Infra.IA;
using Infra.PegasusApi;
using Infra.PegasusApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockExams.Api.Filters;
using MockExams.Infra.Sms;
using MockExams.Infra.UrlShortener;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MockExams.Api.Controllers;

[Route("api/[controller]")]
[EnableCors("AllowAllHeaders")]
public class OperationsController : ControllerBase
{
    protected string _validToken;
    private readonly IWebHostEnvironment _env;
    protected ISmsService _sms;
    protected ILogger<OperationsController> _logger;
    protected IIAClient _chatGptClient;
    protected IUrlShortener _urlShortener;
    protected IPegasusApiClient _pegasusApiClient;

    public OperationsController(IOptions<ServerSettings> settings, IWebHostEnvironment env, ISmsService sms, ILogger<OperationsController> logger, IIAClient chatGptClient, IUrlShortener urlShortener, IPegasusApiClient pegasusApiClient)
    {
        _env = env;
        _sms = sms;
        _logger = logger;
        _chatGptClient = chatGptClient;
        _urlShortener = urlShortener;
        _pegasusApiClient = pegasusApiClient;
    }

    [HttpGet]
    [Authorize("Bearer")]
    [PegasusAuthorizationFilter("Admin")]
    [Route("ForceException")]
    public IActionResult ForceException()
    {
        var teste = 1 / Convert.ToInt32("Teste");
        return BadRequest();
    }

    [HttpGet("Ping")]
    public IActionResult Ping()
    {
        _logger.LogInformation("Log 01 - sdsaldj dljsdlajd");
        _logger.LogWarning("Log 02 - sldajasjdasjjd ddsad");
        _logger.LogError("Log 03 - sdçlsakdç skd kçd ç dkdsçkçlk");

        var result = new
        {
            Service = Assembly.GetEntryAssembly().GetName().Name.ToString(),
            Version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
            DotNetVersion = System.Environment.Version.ToString(),
            Env = _env.EnvironmentName,
            TimeZone = TimeZoneInfo.Local.DisplayName,
            System.Runtime.InteropServices.RuntimeInformation.OSDescription,
            ServerNow = DateTime.Now,
        };
        return Ok(result);
    }

    [HttpPost("SmsTest")]
    [Authorize("Bearer")]
    [PegasusAuthorizationFilter("Admin")]
    public IActionResult SmsTest([FromQuery] string phone)
    {
        if (string.IsNullOrEmpty(phone))
            return BadRequest("Favor informar um número de telefone.");

        Random random = new Random();
        int numeroAleatorio = random.Next(1, 101);

        _sms.SendMessage(phone, $"Teste de SMS {numeroAleatorio}").Wait();
        return Ok();
    }

    [HttpPost("chatgpt-test")]
    [Authorize("Bearer")]
    [PegasusAuthorizationFilter("Admin")]
    public async Task<IActionResult> ChatGptTest([FromQuery] string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
            return BadRequest("Favor informar um prompt.");

        var result = await _chatGptClient.GenerateAsync(prompt);
        return Ok(result);
    }

    [HttpPost("url-shortener-test")]
    [Authorize("Bearer")]
    [PegasusAuthorizationFilter("Admin")]
    public async Task<IActionResult> UrlShortenerTest([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
            return BadRequest("Favor informar uma url.");

        var result = _urlShortener.GetShortUrl(url);
        return Ok(result);
    }

    [HttpPost("pegasus-contact-us-test")]
    [Authorize("Bearer")]
    [PegasusAuthorizationFilter("Admin")]
    public async Task<IActionResult> PegasusContactUsTest([FromQuery] string name, [FromQuery] string email, [FromQuery] string message)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            return BadRequest("Favor informar nome, email e mensagem.");

        var request = new AdminNotificationRequest
        {
            App = "mock-exams",
            Name = name,
            Email = email,
            Phone = "22 988317391", // Telefone válido para teste
            Business = "",
            Message = $"[TESTE] {message}"
        };

        var result = await _pegasusApiClient.NotifyAdminsAsync(request);
        return Ok(result);
    }

    protected bool _IsValidJobToken() => Request.Headers["Authorization"].ToString() == _validToken;
}
