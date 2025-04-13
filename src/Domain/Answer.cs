using Domain.Common;
using System;

namespace Domain;

public class Answer : BaseEntity
{
    public Guid ExamAttemptId { get; set; }
    public Guid? QuestionId { get; set; }
    public string SelectedOptions { get; set; } // Pode armazenar múltiplas respostas como JSON ou string separada por vírgula
    public bool? IsCorrect { get; set; }

    // Relacionamentos
    public virtual ExamAttempt ExamAttempt { get; set; }
    public virtual Question Question { get; set; }
}
