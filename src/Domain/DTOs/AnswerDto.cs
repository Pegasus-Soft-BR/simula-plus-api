using Domain.DTOs;
using System;

namespace Domain.DTOs;

public class AnswerDto
{
    public Guid? QuestionId { get; set; }
    public string SelectedOptions { get; set; } // exemplos válidos: "1" ou "1,2" 
}
