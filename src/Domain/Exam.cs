using Domain.Common;
using MockExams.Helper.Extensions;
using System.Collections.Generic;

namespace Domain;

public class Exam : BaseEntity
{
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            SearchText = value.ToNormalizedSearchText();
        }
    }
    public string Description { get; set; }
    public int TimeSpentMaxSeconds { get; set; }
    public int TotalQuestionsPerAttempt { get; set; }
    public string ImageSlug { get; set; }
    public string SearchText { get; set; }

    public virtual IList<Question> Questions { get; set; }

    public string GetImageUrl(string baseUrl)
    {
        if (string.IsNullOrEmpty(ImageSlug))
            return null;
        else
            return $"{baseUrl}/ExamCovers/{this.ImageSlug}.jpg";
    }
}
