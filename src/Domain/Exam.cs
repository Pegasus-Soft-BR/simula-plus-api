using Domain.Common;
using System.Collections;
using System.Collections.Generic;

namespace Domain;

public class Exam : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int TimeSpentMaxSeconds { get; set; }
    public int TotalQuestionsPerAttempt { get; set; }
    public string ImageSlug { get; set; }

    public virtual IList<Question> Questions { get; set; }

    public string GetImageUrl(string baseUrl)
    {
        if(string.IsNullOrEmpty(ImageSlug))
            return null;
        else 
            return $"{baseUrl}/ExamCovers/{this.ImageSlug}.jpg";
    }
}
