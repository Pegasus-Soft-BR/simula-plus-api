using Domain.DTOs;
using System;

namespace Domain.DTOs;

public class QuestionDto
{
    public string Title { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public string Option4 { get; set; }
    public string Option5 { get; set; }
    public int DifficultyLevel { get; set; }

    public Guid Id { get; set; }
}
