using Domain.Enums;
using System;

namespace Domain.DTOs.Exam;

public class MyExamAttemptDto : BaseDTO
{
    public Guid ExamId { get; set; }
    public string ExamTitle { get; set; }

    public int? TimeSpentSeconds { get; set; }
    public int? Score { get; set; }
    public DateTime? FinishedAt { get; set; }

    public string ImageUrl { get; set; }
    public ExamAttemptStatus Status { get; set; }
}
