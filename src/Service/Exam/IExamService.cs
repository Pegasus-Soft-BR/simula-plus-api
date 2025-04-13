using Domain;
using Domain.DTOs;
using Domain.DTOs.Exam;
using MockExams.Service.Generic;
using System;
using System.Collections.Generic;

namespace MockExams.Service;

public interface IExamService : IBaseService<Exam>
{
    
    StartExamAttemptDto StartExamAttempt(Guid? userId, Guid examId);

    ExamAttemptDto FinishExamAttempt(Guid? userId, FinishExamAttemptDto finishDto);
    IList<MyExamAttemptDto> MyExamAttempts(Guid? userId);
    MyExamAttemptDetailsDto MyExamAttemptDetails(Guid? userId, Guid attemptId);
}
