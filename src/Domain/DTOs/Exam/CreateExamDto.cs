using System.Collections.Generic;

namespace Domain.DTOs.Exam;

public class CreateExamDto : BaseDTO
{
    public string Title { get; set; } // Precisa se algo chamativo como "Fundamentos do C#" por exemplo.
    public string Description { get; set; }

    public IList<CreateExamQuestionDto> Questions { get; set; } // Precisa ser uma lista de perguntas, não pode ser só uma pergunta.

}

public class CreateExamQuestionDto
{
    public string Title { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public string Option4 { get; set; }
    public string Option5 { get; set; }
    public string CorrectOptions { get; set; } // Exemplos válidos "1" ou "1,2"
    public int DifficultyLevel { get; set; } // 1 = Fácil, 2 = Médio, 3 = Difícil

}
