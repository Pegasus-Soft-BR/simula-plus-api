using Domain.Common;
using MockExams.Helper.Extensions;
using System.Collections.Generic;

namespace Domain;

public class Exam : BaseEntity
{
    private string _title;
    private string _description;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            UpdateSearchText();
        }
    }
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            UpdateSearchText();
        }
    }
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

    private void UpdateSearchText()
    {
        SearchText = Title.ToNormalizedSearchText();
        SearchText += " " + Description.ToNormalizedSearchText();
    }
}
