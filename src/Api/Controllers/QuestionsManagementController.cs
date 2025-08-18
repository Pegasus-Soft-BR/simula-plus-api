using AutoMapper;
using Domain;
using Domain.DTOs.Exam;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockExams.Service;

namespace Api.Controllers;

[Route("api/questions-management")]
public class QuestionsManagementController : BaseCrudController<Question, QuestionDto>
{
    public QuestionsManagementController(IQuestionService service, IMapper mapper, ILogger<Question> logger)
        : base(service, mapper, logger)
    {
    }
}