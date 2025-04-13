using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.DTOs.Exam;

// Uusário iniciou um teste.Precisa desses dados para saber o que mostrar na tela.
public class StartExamAttemptDto : BaseDTO
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public ExamAttemptStatus Status { get; set; } = ExamAttemptStatus.InProgress;
    public virtual IList<QuestionDto> Questions { get; set; }
}
