using AutoMapper;
using Domain;
using Domain.DTOs.Exam;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockExams.Service;

namespace Api.Controllers;

[Route("api/exams-management")]
public class ExamsManagementController : BaseCrudController<Exam, ExamDto>
{
    public ExamsManagementController(IExamService service, IMapper mapper, ILogger<Exam> logger)
        : base(service, mapper, logger)
    {
    }

}
