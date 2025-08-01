using Domain.DTOs;
using Infra.IA;
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

    public OperationsController(IOptions<ServerSettings> settings, IWebHostEnvironment env, ISmsService sms, ILogger<OperationsController> logger, IIAClient chatGptClient, IUrlShortener urlShortener)
    {
        _env = env;
        _sms = sms;
        _logger = logger;
        _chatGptClient = chatGptClient;
        _urlShortener = urlShortener;
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

    protected bool _IsValidJobToken() => Request.Headers["Authorization"].ToString() == _validToken;
}
