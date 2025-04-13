using System.Threading.Tasks;

namespace MockExams.Infra.Email
{
    public interface IEmailTemplate
    {
        Task<string> GenerateHtmlFromTemplateAsync(string template, object model);
    }
}
