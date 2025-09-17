using Domain.DTOs;
using Infra.Database;
using Infra.IA;
using Infra.PegasusApi;
using Infra.PegasusApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockExams.Api.Filters;
using MockExams.Helper.Extensions;
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
    private readonly IConfiguration _config;
    protected ILogger<OperationsController> _logger;
    protected IIAClient _chatGptClient;
    protected IPegasusApiClient _pegasusApiClient;
    protected readonly ApplicationDbContext _ctx;

    public OperationsController(IConfiguration config, IOptions<ServerSettings> settings, IWebHostEnvironment env, ILogger<OperationsController> logger, IIAClient chatGptClient, IPegasusApiClient pegasusApiClient, ApplicationDbContext ctx)
    {
        _config = config;
        _env = env;
        _logger = logger;
        _chatGptClient = chatGptClient;
        _pegasusApiClient = pegasusApiClient;
        _ctx = ctx;
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
        var result = new
        {
            DatabaseProvider = _config["DatabaseProvider"],
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

    [HttpPost("backfill-search-text")]
    [Authorize("Bearer")]
    [PegasusAuthorizationFilter("Admin")]
    public async Task<IActionResult> BackfillSearchText()
    {

        try
        {
            var products = await _ctx.Exams.ToListAsync();
            foreach (var p in products)
                p.SearchText = p.Title.ToNormalizedSearchText() + " " + p.Description.ToNormalizedSearchText();
            await _ctx.SaveChangesAsync();
            return Ok("Texto de busca atualizado para todos os produtos.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    protected bool _IsValidJobToken() => Request.Headers["Authorization"].ToString() == _validToken;
}
