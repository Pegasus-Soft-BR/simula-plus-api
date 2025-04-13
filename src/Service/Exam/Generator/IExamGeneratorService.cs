using System.Threading.Tasks;
using Domain;

namespace Service.Exam.Generator;

public interface IExamGeneratorService
{
    Task<Domain.Exam> GenerateAsync(string term);
}
