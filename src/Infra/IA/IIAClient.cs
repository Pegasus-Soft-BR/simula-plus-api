using System.Threading.Tasks;

namespace Infra.IA;

public interface IIAClient
{
    Task<string> GenerateAsync(string prompt);
}
