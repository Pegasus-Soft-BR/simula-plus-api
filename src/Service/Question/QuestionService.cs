using AutoMapper;
using Domain;
using Domain.DTOs.Exam;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MockExams.Infra.Database;
using MockExams.Infra.Database.UoW;
using MockExams.Service.Generic;

namespace MockExams.Service;

public class QuestionService : BaseService<Question, QuestionDto>, IQuestionService
{
    protected ILogger<QuestionService> _logger;


    public QuestionService(ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IValidator<Question> validator, IMapper mapper,
        ILogger<QuestionService> logger) : base(context, unitOfWork, validator, mapper)
    {
        _logger = logger;
    }


}