using Domain.Enums;
using Domain;
using Domain.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Domain;

public class ExamAttempt : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public ExamAttemptStatus Status { get; set; } = ExamAttemptStatus.InProgress;


    // Essas propriedades são preenchidas ao finalizar o exame
    public int? TimeSpentSeconds { get; set; } 
    public int? Score { get; set; } 
    public DateTime? FinishedAt { get; set; }


    // Relacionamentos
    public virtual User User { get; set; }
    public virtual Exam Exam { get; set; }

    public virtual IList<Answer> Answers { get; set; }
}
