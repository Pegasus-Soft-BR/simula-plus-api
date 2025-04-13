using Domain.Common;

namespace Domain;

public class Exam : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int TimeSpentMaxSeconds { get; set; }
    public int TotalQuestionsPerAttempt { get; set; }
    public string ImageSlug { get; set; }

    public string GetImageUrl(string baseUrl)
    {
        return $"{baseUrl}/ExamCovers/{this.ImageSlug}.jpg";
    }
}
