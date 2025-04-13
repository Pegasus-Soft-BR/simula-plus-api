namespace Domain.DTOs.Exam;

public class ExamDto : BaseDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int TimeSpentMaxSeconds { get; set; }
    public int TotalQuestionsPerAttempt { get; set; }
    public string ImageSlug { get; set; }
    public string ImageUrl { get; set; }
}
