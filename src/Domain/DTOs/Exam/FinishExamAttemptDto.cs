using System;
using System.Collections.Generic;

namespace Domain.DTOs.Exam;

public class FinishExamAttemptDto
{
    public Guid ExamAttemptId { get; set; }
    public virtual IList<AnswerDto> Answers { get; set; }
}
