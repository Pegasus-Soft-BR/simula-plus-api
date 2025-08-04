using Domain;
using Domain.Common;
using Domain.DTOs;
using Domain.DTOs.Exam;
using MockExams.Service.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MockExams.Service;

public interface IExamService : IBaseService<Exam>
{

    StartExamAttemptDto StartExamAttempt(Guid? userId, Guid examId);

    ExamAttemptDto FinishExamAttempt(Guid? userId, FinishExamAttemptDto finishDto);
    Task<PagedList<MyExamAttemptDto>> MyExamAttemptsAsync(Guid? userId, int itemsPerPage, int page);
    MyExamAttemptDetailsDto MyExamAttemptDetails(Guid? userId, Guid attemptId);
    Task<List<ExamDto>> Search(string term = "");
}
