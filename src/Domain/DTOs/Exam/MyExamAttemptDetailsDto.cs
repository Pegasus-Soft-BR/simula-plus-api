using Domain.DTOs.Exam;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.DTOs;

public class MyExamAttemptDetailsDto : BaseDTO
{
    public Guid ExamId { get; set; }
    public string ExamTitle { get; set; }

    public int? TimeSpentSeconds { get; set; }
    public int? Score { get; set; }
    public DateTime? FinishedAt { get; set; }

    public string ImageUrl { get; set; }

    public IList<QuestionWithAnswerDto> QuestionWithAnswer { get; set; }
    public ExamAttemptStatus Status { get; set; }
}
