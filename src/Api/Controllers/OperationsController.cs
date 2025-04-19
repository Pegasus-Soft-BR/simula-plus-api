using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockExams.Api.Filters;
using Domain.DTOs;
using MockExams.Infra.Email;
using MockExams.Infra.Sms;
using MockExams.Jobs;
using MockExams.Service.Authorization;
using System;
using System.Reflection;
using Infra.IA;
using System.Threading.Tasks;
using MockExams.Infra.UrlShortener;

namespace MockExams.Api.Controllers;

[Route("api/[controller]")]
[EnableCors("AllowAllHeaders")]
public class OperationsController : ControllerBase
{

    protected IJobExecutor _executor;
    protected string _validToken;
    IEmailService _emailService;
    private readonly IWebHostEnvironment _env;
    protected ISmsService _sms;
    protected ILogger<OperationsController> _logger;
    protected IIAClient _chatGptClient;
    protected IUrlShortener _urlShortener;

    public OperationsController(IJobExecutor executor, IOptions<ServerSettings> settings, IEmailService emailService, IWebHostEnvironment env, ISmsService sms, ILogger<OperationsController> logger, IIAClient chatGptClient, IUrlShortener urlShortener)
    {
        _executor = executor;
        _validToken = settings.Value.JobExecutorToken;
        _emailService = emailService;
        _env = env;
        _sms = sms;
        _logger = logger;
        _chatGptClient = chatGptClient;
        _urlShortener = urlShortener;
    }

    [HttpGet]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
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

    [HttpGet("JobExecutor")]
    [Throttle(Name = "JobExecutor", Seconds = 5, VaryByIp = false)]
    public IActionResult Executor()
    {
        if (!_IsValidJobToken())
            return Unauthorized();
        else
            return Ok(_executor.Execute());
    }

    [HttpPost("EmailTest")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public IActionResult EmailTest([FromBody] EmailTestDTO emailVM)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _emailService.Test(emailVM.Email, emailVM.Name).Wait();
        return Ok();
    }

    [HttpPost("SmsTest")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
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
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public async Task<IActionResult> ChatGptTest([FromQuery] string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
            return BadRequest("Favor informar um prompt.");

        var result = await _chatGptClient.GenerateAsync(prompt);
        return Ok(result);
    }

    [HttpPost("url-shortener-test")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public async Task<IActionResult> UrlShortenerTest([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
            return BadRequest("Favor informar uma url.");

        var result = _urlShortener.GetShortUrl(url);
        return Ok(result);
    }

    protected bool _IsValidJobToken() => Request.Headers["Authorization"].ToString() == _validToken;
}
