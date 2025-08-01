using AutoMapper;
using Domain.Common;
using Domain.DTOs;
using Domain.DTOs.Exam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockExams.Api.Controllers;
using MockExams.Api.Extensions;
using MockExams.Api.Filters;
using MockExams.Service;
using MockExams.Service.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers;

[Route("api/[controller]")]
[EnableCors("AllowAllHeaders")]
[ApiController]
public class ExamsController : ControllerBase
{
    private ILogger<OperationsController> logger;
    private readonly IExamService _service;
    private readonly AutoMapper.IMapper _mapper;

    public ExamsController(ILogger<OperationsController> logger, IExamService service, IMapper mapper)
    {
        this.logger = logger;
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("list-exams")]
    public IActionResult GetExams([FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1, [FromQuery] string order = "CreatedAt desc", [FromQuery] string filter = "")
    {
        var exams = _service.PagedList(itemsPerPage, page, order, filter);
        var examsDto = _mapper.Map<PagedList<ExamDto>>(exams);
        return Ok(examsDto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term = "")
    {
        var examsDto = await _service.Search(term);
        return Ok(examsDto);
    }

    [HttpPost("start-exam-atempt")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("usuario")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StartExamAttemptDto))]
    public IActionResult StartExamAttempt([FromQuery] Guid ExamId)
    {
        var userId = GetCurrentUserId();
        var startDto = _service.StartExamAttempt(userId, ExamId);
        return Ok(startDto);
    }

    [HttpPost("finish-exam-atempt")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("usuario")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamAttemptDto))]
    public IActionResult FinishExamAttempt([FromBody] FinishExamAttemptDto finishDto)
    {
        var userId = GetCurrentUserId();
        ExamAttemptDto results = _service.FinishExamAttempt(userId, finishDto);
        return Ok(results);
    }

    [HttpGet("my-exam-attempts")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("usuario")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MyExamAttemptDto))]
    public IActionResult MyExamAttempts()
    {
        var userId = GetCurrentUserId();
        var results = _service.MyExamAttempts(userId);
        return Ok(results);
    }

    [HttpGet("my-exam-attempt-details")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("usuario")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MyExamAttemptDetailsDto))]
    public IActionResult MyExamAttemptDetail([FromQuery] Guid attemptId)
    {
        var userId = GetCurrentUserId();
        MyExamAttemptDetailsDto results = _service.MyExamAttemptDetails(userId, attemptId);
        return Ok(results);
    }

    [HttpGet("get-enums")]
    public IActionResult GetEnums()
    {
        var profiles = new List<dynamic>();
        foreach (var profileType in Enum.GetValues(typeof(Domain.Enums.Profile)))
        {
            profiles.Add(new
            {
                Value = (int)profileType,
                Text = profileType.ToString()
            });
        }

        var examAttemptStatus = new List<dynamic>();
        foreach (var profileType in Enum.GetValues(typeof(Domain.Enums.ExamAttemptStatus)))
        {
            examAttemptStatus.Add(new
            {
                Value = (int)profileType,
                Text = profileType.ToString()
            });
        }

        var result = new
        {
            profiles,
            examAttemptStatus
        };
        return Ok(result);
    }

    private Guid? GetCurrentUserId()
    {
        return User.GetUserId();
    }
}
