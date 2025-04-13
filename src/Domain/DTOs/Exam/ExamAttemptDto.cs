using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.DTOs.Exam;

public class ExamAttemptDto : BaseDTO
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public ExamAttemptStatus Status { get; set; } = ExamAttemptStatus.InProgress;


    // Essas propriedades são preenchidas ao finalizar o exame
    public int? TimeSpentSeconds { get; set; }
    public int? Score { get; set; }
    public DateTime? FinishedAt { get; set; }

}
