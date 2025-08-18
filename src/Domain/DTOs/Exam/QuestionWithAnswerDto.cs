using System;

namespace Domain.DTOs.Exam;

public class QuestionWithAnswerDto
{
    public Guid QuestionId { get; set; }
    public string Title { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public string Option4 { get; set; }
    public string Option5 { get; set; }
    public string CorrectOptions { get; set; }
    public string SelectedOptions { get; set; }
    public bool IsCorrect { get; set; }
    public bool Multiple { get; set; }
}
