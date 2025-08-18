using Domain;
using Domain.DTOs.Exam;
using MockExams.Service.Generic;

namespace MockExams.Service;

public interface IQuestionService : IBaseService<Question, QuestionDto>
{
}
